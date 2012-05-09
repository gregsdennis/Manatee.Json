/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		XmlExtentions.cs
	Namespace:		Manatee.Json.Extensions
	Class Name:		XmlExtensions
	Purpose:		XML conversions for Manatee.Json

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Manatee.Json.Extensions
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

		/// <summary>
		/// Converts a JsonValue to an XElement
		/// </summary>
		/// <param name="json">A JsonValue.</param>
		/// <param name="key">(optional) The key to be used as a top-level element name.</param>
		/// <returns>An XElement representation of the JsomValue.</returns>
		public static XElement ToXElement(this JsonValue json, string key)
		{
			//var name = FormatKeyToXml(key);
			if (string.IsNullOrWhiteSpace(key) && (json.Type != JsonValueType.Object))
				throw new ArgumentException(EncodingWithoutKeyError);
			XElement xml;
			switch (json.Type)
			{
				case JsonValueType.Number:
					return new XElement(key, json.Number);
				case JsonValueType.String:
					xml = new XElement(key, json.String);
					if (RequiresTypeAttribute(json.String))
						xml.SetAttributeValue(TypeAttribute, "String");
					return xml;
				case JsonValueType.Boolean:
					return new XElement(key, json.Boolean);
				case JsonValueType.Object:
					if (key == null)
					{
						if (json.Object.Count != 1)
							throw new ArgumentException(EncodingWithoutKeyError);
						var kvp = json.Object.ElementAt(0);
						return kvp.Value.ToXElement(kvp.Key);
					}
					xml = new XElement(key);
					foreach (var kvp in json.Object)
					{
						var element = kvp.Value.ToXElement(kvp.Key);
						if (kvp.Value.Type == JsonValueType.Array)
							xml.Add(element.Elements());
						else
							xml.Add(element);
					}
					return xml;
				case JsonValueType.Array:
					if (ContainsAttributeList(json.Array))
					{
						var attributes = json.Array[0].Object;
						xml = json.Array[1].ToXElement(key);
						foreach (var attribute in attributes)
						{
							var value = attribute.Value.ToXElement(key).Value;
							xml.SetAttributeValue(attribute.Key.Substring(1), value);
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
								var element = new XElement(key, xml.Elements());
								element.SetAttributeValue(NestAttribute, true);
								list.Add(element);
								break;
							default:
								list.Add(xml);
								break;
						}
					}
					return new XElement(key, list);
				case JsonValueType.Null:
					return new XElement(key);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		/// <summary>
		/// Converts an XElement to a JsonObject.
		/// </summary>
		/// <param name="xElement">An XElement.</param>
		/// <returns>The JsonValue representation of the XElement.</returns>
		public static JsonValue ToJson(this XElement xElement)
		{
			return new JsonObject {{xElement.Name.ToString(), GetValue(xElement)}};
		}
		/// <summary>
		/// Converts an XElement to a JsonObject.
		/// </summary>
		/// <param name="xElements">A collection of XElement objects.</param>
		/// <returns>A single JsonValue which represents the list of XElement objects.</returns>
		public static JsonValue ToJson(this IEnumerable<XElement> xElements)
		{
			var json = new JsonObject();
			foreach (var xElement in xElements)
			{
				var name = xElement.Name.ToString();
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
			}
			return json;
		}

		private static bool RequiresTypeAttribute(string value)
		{
			double d;
			var s = value.ToLower();
			return (s == "true") || (s == "false") || (s == "null") || double.TryParse(s, out d);
		}
		private static string FormatKeyToXml(string key)
		{
			throw new NotImplementedException();
		}
		private static string FormatKeyFromXml(string key)
		{
			throw new NotImplementedException();
		}
		private static JsonValue GetValue(XElement xElement)
		{
			var typeAttribute = xElement.Attribute(TypeAttribute);
			var otherAttributes = xElement.Attributes().Where(a => (a.Name != NestAttribute) && (a.Name != TypeAttribute));
			if (xElement.HasElements)
				return AttachAttributes(xElement.Elements().ToJson(), otherAttributes);
			if (string.IsNullOrEmpty(xElement.Value) && (typeAttribute == null))
				return AttachAttributes(JsonValue.Null, otherAttributes);
			var value = xElement.Value;
			if ((typeAttribute != null) && (typeAttribute.Value.ToLower() == "string"))
				return AttachAttributes(value, otherAttributes);
			return AttachAttributes(ParseValue(value), otherAttributes);
		}
		private static JsonValue AttachAttributes(JsonValue json, IEnumerable<XAttribute> attributes)
		{
			if (attributes == null)
				return json;
			if (attributes.Count() == 0)
				return json;
			var obj = new JsonObject();
			foreach (var xAttribute in attributes)
			{
				obj.Add(string.Format("-{0}",xAttribute.Name), ParseValue(xAttribute.Value));
			}
			return new JsonArray {obj, json};
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

		/// <summary>
		/// Converts an XmlNode to an XElement.
		/// </summary>
		/// <param name="node">An XmlNode.</param>
		/// <returns>The XElement construct of the XmlNode.</returns>
		/// <remarks>Provided for convenience.</remarks>
		public static XElement ToXElement(this XmlNode node)
		{
			var xDoc = new XDocument();
			using (var xmlWriter = xDoc.CreateWriter())
				node.WriteTo(xmlWriter);
			return xDoc.Root;
		}
		/// <summary>
		/// Converts an XElement to an XmlNode.
		/// </summary>
		/// <param name="element">An XElement.</param>
		/// <returns>The XmlNode construct of the XElement.</returns>
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
	}
}
