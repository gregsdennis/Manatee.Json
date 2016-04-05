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
 
	File Name:		AnyOfSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		AnyOfSchema
	Purpose:		Used to define a collection of schema conditions, any number of
					which may be satisfied.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Used to define a collection of schema conditions, any number of which may
	/// be satisfied.
	/// </summary>
	public class AnyOfSchema : JsonSchema
	{
		/// <summary>
		/// A collection of schema options.
		/// </summary>
		public IEnumerable<IJsonSchema> Options { get; set; }

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			var jValue = root ?? ToJson(null);
			var errors = Options.Select(s => s.Validate(json, jValue)).ToList();
			return errors.Any(r => r.Valid)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(errors);
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
			Options = json.Object["anyOf"].Array.Select(JsonSchemaFactory.FromJson);
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
			json.Object.Add("anyOf", Options.ToJson(serializer));
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
			var schema = other as AnyOfSchema;
			return base.Equals(other) && Options.ContentsEqual(schema?.Options);
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
			unchecked
			{
				return (base.GetHashCode()*397) ^ (Options?.GetCollectionHashCode() ?? 0);
			}
		}
	}
}