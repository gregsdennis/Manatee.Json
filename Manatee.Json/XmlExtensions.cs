﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json
{
	/// <summary>
	/// Contains functionality to map JSON values to XML constructs.
	/// </summary>
	public static class XmlExtensions
	{
		private const string EncodingWithoutKeyError = "Key cannot be empty or contain whitespace characters.";
		private const string DecodingNestedArrayWithMismatchedKeysError = "The element name for items in nested arrays must match the element name of its parent.";
		private const string NestAttribute = "nest";
		private const string TypeAttribute = "type";
		private const string XmlNamespaceAttribute = "xmlns";

		/// <summary>
		/// Converts a <see cref="JsonValue"/> to an XElement
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/>.</param>
		/// <param name="key">The key to be used as a top-level element name.</param>
		/// <returns>An <see cref="XElement"/> representation of the <see cref="JsonValue"/>.</returns>
		/// <remarks>
		/// The 'key' parameter may be null only when the underlying JSON is an
		/// object which contains a single key/value pair.
		/// </remarks>
		/// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is null, empty, or whitespace
		/// and <paramref name="json"/> is not a non-empty <see cref="JsonObject"/>.</exception>
		public static XElement ToXElement(this JsonValue json, string key)
		{
			if (key.IsNullOrWhiteSpace() && (json.Type != JsonValueType.Object))
				throw new ArgumentException(EncodingWithoutKeyError);
			var name = GetXName(key);
			XElement xml;
			switch (json.Type)
			{
				case JsonValueType.Number:
					return new XElement(name, json.Number);
				case JsonValueType.String:
					xml = new XElement(name, json.String);
					if (RequiresTypeAttribute(json.String))
						xml.SetAttributeValue(TypeAttribute, "String");
					return xml;
				case JsonValueType.Boolean:
					return new XElement(name, json.Boolean);
				case JsonValueType.Object:
					if (name == null)
					{
						if (json.Object.Count != 1)
							throw new ArgumentException(EncodingWithoutKeyError);
						var kvp = json.Object.ElementAt(0);
						return kvp.Value.ToXElement(kvp.Key);
					}
					xml = new XElement(name);
					foreach (var kvp in json.Object)
					{
						var element = kvp.Value.ToXElement(kvp.Key);
						if ((kvp.Value.Type == JsonValueType.Array) && !ContainsAttributeList(kvp.Value.Array))
							xml.Add(element.Elements());
						else
							xml.Add(element);
					}
					return xml;
				case JsonValueType.Array:
					if (ContainsAttributeList(json.Array))
					{
						var attributeNames = json.Array[0].Object;
						var attributes = new List<XAttribute>();
						foreach(var attributeName in attributeNames)
						{
							var localName = GetXName(attributeName.Key.Substring(1));
							var attribute = new XAttribute(localName, attributeName.Value.ToXElement(key).Value);
							if (attribute.IsNamespaceDeclaration)
								XmlNamespaceRegistry.Instance.Register(attribute.Name.LocalName,attribute.Value);
							attributes.Add(attribute);
						}
						xml = json.Array[1].ToXElement(key);
						foreach (var attribute in attributes)
						{
							if (attribute.IsNamespaceDeclaration)
							{
								if (attribute.Name.LocalName == XmlNamespaceAttribute)
								{
									XNamespace ns = XmlNamespaceRegistry.Instance.GetNamespace(XmlNamespaceAttribute);
									xml.Name = ns + xml.Name?.LocalName;
								}
								else
									xml.Add(attribute);
								XmlNamespaceRegistry.Instance.Unregister(attribute.Name.LocalName);
							}
							else
							{
								xml.Add(attribute);
							}
						}
						return xml;
					}
					var list = new List<XElement>();
					foreach (var jv in json.Array)
					{
						xml = jv.ToXElement(key);
						switch (jv.Type)
						{
							case JsonValueType.Array:
								var element = new XElement(name, xml.Elements());
								element.SetAttributeValue(NestAttribute, true);
								list.Add(element);
								break;
							default:
								list.Add(xml);
								break;
						}
					}
					return new XElement(name, list);
				case JsonValueType.Null:
					return new XElement(name);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		/// <summary>
		/// Converts an <see cref="XElement"/> to a <see cref="JsonObject"/>.
		/// </summary>
		/// <param name="xElement">An <see cref="XElement"/>.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the <see cref="XElement"/>.</returns>
		public static JsonValue ToJson(this XElement xElement)
		{
			return new JsonObject {{GetNamespaceForElement(xElement) + xElement.Name.LocalName, GetValue(xElement)}};
		}
		/// <summary>
		/// Converts an <see cref="XElement"/> to a <see cref="JsonObject"/>.
		/// </summary>
		/// <param name="xElements">A collection of <see cref="XElement"/> objects.</param>
		/// <returns>A single <see cref="JsonValue"/> which represents the list of <see cref="XElement"/> objects.</returns>
		/// <exception cref="XmlException">Thrown if an error occurs while attempting to convert an array of elements.</exception>
		public static JsonValue ToJson(this IEnumerable<XElement> xElements)
		{
			var json = new JsonObject();
			foreach (var xElement in xElements)
			{
				XmlNamespaceRegistry.Instance.RegisterElement(xElement);
				var name = GetNamespaceForElement(xElement) + xElement.Name.LocalName;
				var newValue = GetValue(xElement);
				if (json.ContainsKey(name))
				{
					var item = json[name];
					var nestAttribute = xElement.Attribute(NestAttribute);
					if ((nestAttribute != null) && (nestAttribute.Value.ToLower() == "true"))
					{
						if ((newValue.Object.Count > 1) || ((newValue.Object.Count != 0) && (newValue.Object.Keys.ElementAt(0) != name)))
							throw new XmlException(DecodingNestedArrayWithMismatchedKeysError);
						newValue = newValue.Object[name].Type == JsonValueType.Array
						           	? newValue.Object[name]
						           	: new JsonArray {newValue.Object[name]};
					}
					if (item.Type == JsonValueType.Array)
						item.Array.Add(newValue);
					else
						json[name] = new JsonArray { item, newValue };
				}
				else
				{
					json[name] = newValue;
				}
				XmlNamespaceRegistry.Instance.UnRegisterElement(xElement);
			}
			return json;
		}

		private static bool RequiresTypeAttribute(string value)
		{
			double d;
			var s = value.ToLower();
			return (s == "true") || (s == "false") || (s == "null") || double.TryParse(s, out d);
		}
		private static JsonValue GetValue(XElement xElement)
		{
			var typeAttribute = xElement.Attribute(TypeAttribute);
			if (xElement.HasElements)
				return AttachAttributes(xElement.Elements().ToJson(), xElement);
			if (string.IsNullOrEmpty(xElement.Value) && (typeAttribute == null))
				return AttachAttributes(JsonValue.Null, xElement);
			var value = xElement.Value;
			if ((typeAttribute != null) && (typeAttribute.Value.ToLower() == "string"))
				return AttachAttributes(value, xElement);
			return AttachAttributes(ParseValue(value), xElement);
		}
		private static JsonValue AttachAttributes(JsonValue json, XElement xElement)
		{
			var attributes = xElement.Attributes().Where(a => (a.Name != NestAttribute) && (a.Name != TypeAttribute)).ToList();
			if (attributes.Count == 0)
				return json;
			var obj = new JsonObject();
			foreach (var xAttribute in attributes)
			{
				var name = xAttribute.IsNamespaceDeclaration && (xAttribute.Name.LocalName != XmlNamespaceAttribute)
							? $"{XmlNamespaceAttribute}:{xAttribute.Name.LocalName}"
					           : GetNamespaceForElement(xElement, xAttribute.Name.NamespaceName) + xAttribute.Name.LocalName;
				obj.Add($"-{name}", ParseValue(xAttribute.Value));
			}
			return new JsonArray { obj, json };
		}
		private static JsonValue ParseValue(string value)
		{
			bool b;
			if (bool.TryParse(value, out b))
				return b;
			double d;
			if (double.TryParse(value, out d))
				return d;
			return value;
		}
		private static bool ContainsAttributeList(JsonArray json)
		{
			if (json.Count != 2) return false;
			if (json[0].Type != JsonValueType.Object) return false;
			return json[0].Object.Keys.All(key => key[0] == '-');
		}
		private static string GetNamespaceForElement(XElement xElement, string space = null)
		{
			var search = space ?? xElement.Name.NamespaceName;
			if (string.IsNullOrEmpty(search)) return string.Empty;
			var parent = xElement;
			while (parent != null)
			{
				if (XmlNamespaceRegistry.Instance.ElementDefinesNamespace(parent, search))
				{
					var label = XmlNamespaceRegistry.Instance.GetLabel(parent, search);
					return label == XmlNamespaceAttribute
					       	? string.Empty
					       	: $"{XmlNamespaceRegistry.Instance.GetLabel(parent, search)}:";
				}
				parent = parent.Parent;
			}
			return string.Empty;
		}
		private static XName GetXName(string key)
		{
			if (key == null) return null;
			if (!key.Contains(":") && (key != XmlNamespaceAttribute)) return key;
			XName name;
			if (key.Contains(":"))
			{
				var label = key.Substring(0, key.IndexOf(':'));
				var local = key.Substring(label.Length + 1);
				var ns = XmlNamespaceRegistry.Instance.GetNamespace(label == XmlNamespaceAttribute ? local : label) ?? XNamespace.Xmlns;
				name = ns + local;
			}
			else
			{
				name = XName.Get(XmlNamespaceAttribute, string.Empty);
			}
			return name;
		}

#if !IOS && !CORE
		/// <summary>
		/// Converts an <see cref="XmlNode"/> to an <see cref="XElement"/>.
		/// </summary>
		/// <param name="node">An <see cref="XmlNode"/>.</param>
		/// <returns>The <see cref="XElement"/> construct of the <see cref="XmlNode"/>.</returns>
		/// <remarks>Provided for convenience.</remarks>
		public static XElement ToXElement(this XmlNode node)
		{
			var xDoc = new XDocument();
			using (var xmlWriter = xDoc.CreateWriter())
				node.WriteTo(xmlWriter);
			return xDoc.Root;
		}
		/// <summary>
		/// Converts an <see cref="XElement"/> to an <see cref="XmlNode"/>.
		/// </summary>
		/// <param name="element">An <see cref="XElement"/>.</param>
		/// <returns>The <see cref="XmlNode"/> construct of the <see cref="XElement"/>.</returns>
		/// <remarks>Provided for convenience.</remarks>
		public static XmlNode ToXmlNode(this XElement element)
		{
			using (var xmlReader = element.CreateReader())
			{
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlReader);
				return xmlDoc;
			}
		}
		
#endif
		}
}
