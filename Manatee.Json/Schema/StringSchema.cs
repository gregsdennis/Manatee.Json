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
 
	File Name:		StringSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		StringSchema
	Purpose:		Defines a schema which expects a string.

***************************************************************************************/

using System.Collections.Generic;
using System.Text.RegularExpressions;

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
				return new SchemaValidationResults(string.Empty, string.Format("Expected: String; Actual: {0}.", json.Type));
			var str = json.String;
			var length = str.Length;
			var errors = new List<SchemaValidationError>();
			if (MinLength.HasValue && (length < MinLength))
				errors.Add(new SchemaValidationError(string.Empty, string.Format("Expected: length >= {0}; Actual: {1}.", MinLength, length)));
			if (MaxLength.HasValue && (length > MaxLength))
				errors.Add(new SchemaValidationError(string.Empty, string.Format("Expected: length <= {0}; Actual: {1}.", MaxLength, length)));
			if (Format != null && !Format.Validate(str))
				errors.Add(new SchemaValidationError(string.Empty, string.Format("Value [{0}] is not in an acceptable [{1}] format.", str, Format.Key)));
			if (Pattern != null && !Regex.IsMatch(str, Pattern))
				errors.Add(new SchemaValidationError(string.Empty, string.Format("Value [{0}] does not match required Regex pattern [{1}].", str, Pattern)));
			return new SchemaValidationResults(errors);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		public override void FromJson(JsonValue json)
		{
			base.FromJson(json);
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
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson()
		{
			var json = base.ToJson().Object;
			if (Format != null) json["format"] = Format.Key;
			if (MinLength.HasValue) json["minLength"] = MinLength;
			if (MaxLength.HasValue) json["maxLength"] = MaxLength;
			if (Pattern != null) json["pattern"] = Pattern;
			return json;
		}
	}
}