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
 
	File Name:		IntegerSchemaValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		IntegerSchemaValidator
	Purpose:		Validates schemas with number-specific properties.

***************************************************************************************/
using System.Collections.Generic;

namespace Manatee.Json.Schema.Validators
{
	internal class NumberSchemaValidator : IJsonSchemaValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return Equals(schema.Type, JsonSchemaTypeDefinition.Number) ||
			       schema.Maximum.HasValue ||
			       schema.ExclusiveMaximum.HasValue ||
			       schema.Minimum.HasValue ||
			       schema.ExclusiveMinimum.HasValue ||
			       schema.MultipleOf.HasValue;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (Equals(schema.Type, JsonSchemaTypeDefinition.Number))
			{
				if (json.Type != JsonValueType.Number)
					return new SchemaValidationResults(string.Empty, $"Expected: Number; Actual: {json.Type}.");
			}
			var number = json.Number;
			var errors = new List<SchemaValidationError>();
			if (schema.ExclusiveMinimum ?? false)
			{
				if (number <= schema.Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: > {schema.Minimum}; Actual: {number}."));
			}
			else
			{
				if (number < schema.Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: >= {schema.Minimum}; Actual: {number}."));
			}
			if (schema.ExclusiveMaximum ?? false)
			{
				if (number >= schema.Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: < {schema.Maximum}; Actual: {number}."));
			}
			else
			{
				if (number > schema.Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: <= {schema.Maximum}; Actual: {number}."));
			}
			if (schema.MultipleOf.HasValue && number % schema.MultipleOf != 0)
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: {number}%{schema.MultipleOf}=0; Actual: {number % schema.MultipleOf}."));
			return new SchemaValidationResults(errors);
		}
	}
}
