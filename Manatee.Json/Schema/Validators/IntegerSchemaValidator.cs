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
	Purpose:		Validates schemas with integer-specific properties.

***************************************************************************************/
using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal class IntegerSchemaValidator : IJsonSchemaValidator
	{
		public bool Applies(JsonSchema schema)
		{
			// we don't care about the other properties if not an integer.  Number will pick those up.
			return Equals(schema.Type, JsonSchemaTypeDefinition.Integer);
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Type != JsonValueType.Number || !json.Number.IsInt())
				return new SchemaValidationResults(string.Empty, $"Expected: Integer; Actual: {json.Type}.");
			var errors = new List<SchemaValidationError>();
			var integer = (int)json.Number;
			if (schema.ExclusiveMinimum ?? false)
			{
				if (integer <= schema.Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: > {schema.Minimum}; Actual: {integer}."));
			}
			else
			{
				if (integer < schema.Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: >= {schema.Minimum}; Actual: {integer}."));
			}
			if (schema.ExclusiveMaximum ?? false)
			{
				if (integer >= schema.Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: < {schema.Maximum}; Actual: {integer}."));
			}
			else
			{
				if (integer > schema.Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: <= {schema.Maximum}; Actual: {integer}."));
			}
			if (schema.MultipleOf.HasValue && integer % schema.MultipleOf != 0)
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: {integer}%{schema.MultipleOf}=0; Actual: {integer % schema.MultipleOf}."));
			return new SchemaValidationResults(errors);
		}
	}
}
