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
 
	File Name:		JsonSchemaCollection.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaCollection
	Purpose:		Represents a collection of schemata.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Represents a collection of schemata.
	/// </summary>
	public class JsonSchemaCollection : List<IJsonSchema>, IJsonSchema
	{
		public Uri DocumentPath { get; set; }

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			return new SchemaValidationResults(this.Select(s => s.Validate(json, root)));
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchema other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			var collection = other as JsonSchemaCollection;
			return collection != null && this.SequenceEqual(collection);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(j => JsonSchemaFactory.FromJson(j, DocumentPath)));
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonArray(this.Select(s => s.ToJson(serializer)));
		}
	}
}
