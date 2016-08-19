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

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects a string.
	/// </summary>
	public class StringSchema : JsonSchema
	{
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
	}
}