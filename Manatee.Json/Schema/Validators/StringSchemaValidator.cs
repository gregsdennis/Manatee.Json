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
 
	File Name:		StringSchemaValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		StringSchemaValidator
	Purpose:		Validates schemas with string-specific properties.

***************************************************************************************/
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Manatee.Json.Schema.Validators
{
	internal class StringSchemaValidator : IJsonSchemaValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return Equals(schema.Type, JsonSchemaTypeDefinition.String) ||
			       schema.MaxLength.HasValue ||
			       schema.MinLength.HasValue ||
			       schema.Format != null ||
			       schema.Pattern != null;

		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Type != JsonValueType.String)
			{
				if (Equals(schema.Type, JsonSchemaTypeDefinition.String))
					return new SchemaValidationResults(string.Empty, $"Expected: Object; String: {json.Type}.");
				return new SchemaValidationResults();
			}
			var str = json.String;
			var length = str.Length;
			var errors = new List<SchemaValidationError>();
			if (schema.MinLength.HasValue && (length < schema.MinLength))
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: length >= {schema.MinLength}; Actual: {length}."));
			if (schema.MaxLength.HasValue && (length > schema.MaxLength))
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: length <= {schema.MaxLength}; Actual: {length}."));
			if (schema.Format != null && !schema.Format.Validate(str))
				errors.Add(new SchemaValidationError(string.Empty, $"Value [{str}] is not in an acceptable {schema.Format.Key} format."));
			if (schema.Pattern != null && !Regex.IsMatch(str, schema.Pattern))
				errors.Add(new SchemaValidationError(string.Empty, $"Value [{str}] does not match required Regex pattern [{schema.Pattern}]."));
			return new SchemaValidationResults(errors);
		}
	}
}
