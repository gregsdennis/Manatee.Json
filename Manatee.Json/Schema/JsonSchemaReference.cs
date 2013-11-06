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
		/// <param name="reference"></param>
		public JsonSchemaReference(string reference)
		{
			Reference = reference;
		}

		/// <summary>
		/// Resolves the reference in relation to a specific root.
		/// </summary>
		/// <param name="root"></param>
		public void Resolve(JsonValue root)
		{
			if (string.IsNullOrWhiteSpace(Reference))
				throw new ArgumentNullException("Reference");
			if (root == null)
				throw new ArgumentNullException("root");
			if (Reference[0] == '#')
				_schema = ResolveLocalReference(root);
			else
				_schema = ResolveExternalReference();
		}
		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			var jValue = root ?? ToJson();
			if (_schema == null)
				Resolve(jValue);
			return Resolved.Validate(json, jValue);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		public void FromJson(JsonValue json)
		{
			Reference = json.Object["$ref"].String;
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson()
		{
			return new JsonObject {{"$ref", Reference}};
		}

		private IJsonSchema ResolveLocalReference(JsonValue root)
		{
			var properties = Reference.Split('/').Skip(1);
			if (!properties.Any()) return JsonSchemaFactory.FromJson(root);
			var value = root;
			foreach (var property in properties)
			{
				value = value.Object[property];
			}
			return JsonSchemaFactory.FromJson(value);
		}
		private IJsonSchema ResolveExternalReference()
		{
			var schemaJson = new WebClient().DownloadString(Reference);
			return JsonSchemaFactory.FromJson(JsonValue.Parse(schemaJson));
		}
	}
}