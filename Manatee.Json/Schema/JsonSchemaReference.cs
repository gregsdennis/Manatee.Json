/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSchemaReference.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaReference
	Purpose:		Defines a reference to a schema.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Internal;
using Manatee.Json.Path;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a reference to a schema.
	/// </summary>
	public class JsonSchemaReference : JsonSchema
	{
		/// <summary>
		/// Defines a reference to the root schema.
		/// </summary>
		public static readonly JsonSchemaReference Root = new JsonSchemaReference("#");

		private static readonly JsonValue _rootJson = Root.ToJson(null);
		private static readonly Regex _generalEscapePattern = new Regex("%(?<Value>[0-9A-F]{2})", RegexOptions.IgnoreCase);

		/// <summary>
		/// Defines the reference in respect to the root schema.
		/// </summary>
		public string Reference { get; private set; }
		/// <summary>
		/// Exposes the schema at the references location.
		/// </summary>
		/// <remarks>
		/// The <see cref="Resolve"/> method must first be called.
		/// </remarks>
		public IJsonSchema Resolved { get; private set; }

		internal JsonSchemaReference() {}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaReference"/> class.
		/// </summary>
		/// <param name="reference">The relative (internal) or absolute (URI) path to the referenced type definition.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="reference"/> is nulll, empty, or whitespace.</exception>
		public JsonSchemaReference(string reference)
		{
			if (reference.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(reference));
			Reference = reference;
		}
		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			var jValue = root ?? ToJson(null);
			var results = base.Validate(json, jValue);
			if (Resolved == null || root == null)
				jValue = Resolve(jValue, DocumentPath);
			var refResults = Resolved?.Validate(json, jValue) ??
			                 new SchemaValidationResults(null, "Error finding referenced schema.");
			return new SchemaValidationResults(new[] {results, refResults});
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public override void FromJson(JsonValue json, JsonSerializer serializer)
		{
			base.FromJson(json, serializer);
			Reference = json.Object["$ref"].String;
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson(JsonSerializer serializer)
		{
			var json = base.ToJson(serializer);
			json.Object["$ref"] = Reference;
			return json;
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public override bool Equals(IJsonSchema other)
		{
			var schema = other as JsonSchemaReference;
			return schema != null && schema.Reference == Reference;
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			return Reference?.GetHashCode() ?? 0;
		}

		private JsonValue Resolve(JsonValue root)
		{
			return Resolve(root, null);
		}

		private JsonValue Resolve(JsonValue root, Uri documentPath)
		{
			var referenceParts = Reference.Split(new[] {'#'}, StringSplitOptions.None);
			var address = referenceParts[0];
			var path = referenceParts.Length > 1 ? referenceParts[1] : string.Empty;
			var jValue = root;
			if (!address.IsNullOrWhiteSpace())
			{
				Uri uri;
				var search = JsonPathWith.Search("id");
				var allIds = new Stack<string>(search.Evaluate(root ?? new JsonObject()).Select(jv => jv.String));
				while (allIds.Any() && !Uri.TryCreate(address, UriKind.Absolute, out uri))
				{
					address = allIds.Pop() + address;
				}

				Uri absolute = null;

				if (DocumentPath != null && !Uri.TryCreate(address, UriKind.Absolute, out absolute))
				{
					DocumentPath = new Uri(DocumentPath.GetParentUri(), address);
				}

				jValue = JsonSchemaRegistry.Get(DocumentPath?.ToString() ?? address).ToJson(null);
			}
			if (jValue == null) return root;
			if (jValue == _rootJson) throw new ArgumentException("Cannot use a root reference as the base schema.");
 
			Resolved = ResolveLocalReference(jValue, path, DocumentPath);
			Resolved.DocumentPath = documentPath;
			return jValue;
		}
		private static IJsonSchema ResolveLocalReference(JsonValue root, string path, Uri documentPath)
		{
			var properties = path.Split('/').Skip(1).ToList();
			IJsonSchema schema = null;
			if (!properties.Any())
			{
				schema = JsonSchemaFactory.FromJson(root);
				schema.DocumentPath = documentPath;
				schema.FromJson(root, null);
			}
			var value = root;
			foreach (var property in properties)
			{
				var unescaped = Unescape(property);
				if (value.Type == JsonValueType.Object)
				{
					if (!value.Object.ContainsKey(unescaped)) return null;
					value = value.Object[unescaped];
				}
				else if (value.Type == JsonValueType.Array)
				{
					int index;
					if (!int.TryParse(unescaped, out index) || index >= value.Array.Count) return null;
					value = value.Array[index];
				}
			}
			schema = JsonSchemaFactory.FromJson(value);
			schema.DocumentPath = documentPath;
			schema.FromJson(value, null);
			return schema;
		}
		private static string Unescape(string reference)
		{
			var unescaped = reference.Replace("~1", "/")
			                .Replace("~0", "~");
			var matches = _generalEscapePattern.Matches(unescaped);
			foreach (Match match in matches)
			{
				var value = int.Parse(match.Groups["Value"].Value, NumberStyles.HexNumber);
				var ch = (char) value;
				unescaped = Regex.Replace(unescaped, match.Value, new string(ch, 1));
			}
			return unescaped;
		}
	}
}