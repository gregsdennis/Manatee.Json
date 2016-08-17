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
 
	File Name:		StringSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		StringSchema
	Purpose:		Defines a schema which expects a string.

***************************************************************************************/

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects a string.
	/// </summary>
	public class StringSchema : JsonSchema
	{
		/// <summary>
		/// Defines a required format for the string.
		/// </summary>
		public StringFormat Format { get; set; }
		/// <summary>
		/// Defines a minimum acceptable length.
		/// </summary>
		public uint? MinLength { get; set; }
		/// <summary>
		/// Defines a maximum acceptable length.
		/// </summary>
		public uint? MaxLength { get; set; }
		/// <summary>
		/// Defines a <see cref="Regex"/> pattern for to which the value must adhere.
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="StringSchema"/> class
		/// </summary>
		public StringSchema() : base(JsonSchemaTypeDefinition.String) {}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			if (json.Type != JsonValueType.String)
				return new SchemaValidationResults(string.Empty, $"Expected: String; Actual: {json.Type}.");
			var str = json.String;
			var length = str.Length;
			var errors = new List<SchemaValidationError>();
			if (MinLength.HasValue && (length < MinLength))
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: length >= {MinLength}; Actual: {length}."));
			if (MaxLength.HasValue && (length > MaxLength))
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: length <= {MaxLength}; Actual: {length}."));
			if (Format != null && !Format.Validate(str))
				errors.Add(new SchemaValidationError(string.Empty, $"Value [{str}] is not in an acceptable {Format.Key} format."));
			if (Pattern != null && !Regex.IsMatch(str, Pattern))
				errors.Add(new SchemaValidationError(string.Empty, $"Value [{str}] does not match required Regex pattern [{Pattern}]."));
			return new SchemaValidationResults(errors);
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
			var formatKey = obj.TryGetString("format");
			Format = StringFormat.GetFormat(formatKey);
			MinLength = (uint?) obj.TryGetNumber("minLength");
			MaxLength = (uint?) obj.TryGetNumber("maxLength");
			Pattern = obj.TryGetString("pattern");
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson(JsonSerializer serializer)
		{
			var json = base.ToJson(serializer).Object;
			if (Format != null) json["format"] = Format.Key;
			if (MinLength.HasValue) json["minLength"] = MinLength;
			if (MaxLength.HasValue) json["maxLength"] = MaxLength;
			if (Pattern != null) json["pattern"] = Pattern;
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
			var schema = other as StringSchema;
			return base.Equals(schema) &&
			       Format == schema.Format &&
			       MinLength == schema.MinLength &&
			       MaxLength == schema.MaxLength &&
			       Pattern == schema.Pattern;
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
				hashCode = (hashCode*397) ^ (Format?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ MinLength.GetHashCode();
				hashCode = (hashCode*397) ^ MaxLength.GetHashCode();
				hashCode = (hashCode*397) ^ (Pattern?.GetHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}