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
 
	File Name:		OneOfSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		OneOfSchema
	Purpose:		Used to define a collection of schema conditions, exactly
					one of which must be satisfied.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Used to define a collection of schema conditions, exactly one of which must
	/// be satisfied.
	/// </summary>
	public class OneOfSchema : JsonSchema
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
			var validCount = errors.Count(r => r.Valid);
			switch (validCount)
			{
				case 0:
					return new SchemaValidationResults(errors);
				case 1:
					return new SchemaValidationResults();
				default:
					return new SchemaValidationResults(string.Empty, "More than one option was valid.");
			}
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
			var obj = json.Object;
			if (obj.ContainsKey("oneOf"))
			{
				Options = obj["oneOf"].Array.Select(JsonSchemaFactory.FromJson).ToList();
			}
			else if (obj.ContainsKey("type"))
			{
				var array = obj["type"].Array;
				Options = array.Select(JsonSchemaFactory.GetPrimitiveSchema).ToList();
				// The schema may contain requirements for each of the entries in the array.
				// We need to be sure we deserialize those.
				foreach (var schema in Options)
				{
					schema.FromJson(json, null);
				}
			}
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson(JsonSerializer serializer)
		{
			var json = new JsonObject {{"oneOf", Options.ToJson(serializer)}};
			if (Default != null) json["default"] = Default;
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
			var schema = other as OneOfSchema;
			return Options.ContentsEqual(schema?.Options);
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
				int hashCode = base.GetHashCode();
				hashCode = (hashCode*397) ^ (Options?.GetCollectionHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}