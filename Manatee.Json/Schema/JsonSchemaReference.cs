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
 
	File Name:		JsonSchemaReference.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaReference
	Purpose:		Defines a reference to a schema.

***************************************************************************************/

using System;
using System.Linq;
using System.Net;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a reference to a schema.
	/// </summary>
	public class JsonSchemaReference : IJsonSchema
	{
		/// <summary>
		/// Defines a reference to the root schema.
		/// </summary>
		public static readonly JsonSchemaReference Root = new JsonSchemaReference("#");
		private static readonly JsonValue RootJson = Root.ToJson(null);

		private IJsonSchema _schema;

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
		public virtual IJsonSchema Resolved { get { return _schema; } }

		internal JsonSchemaReference() {}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaReference"/> class.
		/// </summary>
		/// <param name="reference">The relative (internal) or absolute (URI) path to the referenced type definition.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="reference"/> is nulll, empty, or whitespace.</exception>
		public JsonSchemaReference(string reference)
		{
			if (reference.IsNullOrWhiteSpace())
				throw new ArgumentNullException("reference");
			Reference = reference;
		}
		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			var jValue = root ?? ToJson(null);
			if (_schema == null)
				Resolve(jValue);
			return Resolved.Validate(json, jValue);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Reference = json.Object["$ref"].String;
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject {{"$ref", Reference}};
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public virtual bool Equals(IJsonSchema other)
		{
			var schema = other as JsonSchemaReference;
			return (schema != null) && (schema.Reference == Reference);
		}

		private void Resolve(JsonValue root)
		{
			if (root == null)
				throw new ArgumentNullException("root");
			if (root == RootJson)
				throw new ArgumentException("Cannot use a root reference as the base schema.");
			_schema = Reference[0] == '#' ? ResolveLocalReference(root) : ResolveExternalReference();
		}
		private IJsonSchema ResolveLocalReference(JsonValue root)
		{
			var properties = Reference.Split('/').Skip(1).ToList();
			if (!properties.Any()) return JsonSchemaFactory.FromJson(root);
			var value = properties.Aggregate(root, (current, property) => current.Object[property]);
			return JsonSchemaFactory.FromJson(value);
		}
		private IJsonSchema ResolveExternalReference()
		{
			var schemaJson = new WebClient().DownloadString(Reference);
			return JsonSchemaFactory.FromJson(JsonValue.Parse(schemaJson));
		}
	}
}