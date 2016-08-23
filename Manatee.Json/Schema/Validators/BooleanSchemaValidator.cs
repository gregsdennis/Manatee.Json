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
 
	File Name:		BooleanSchemaValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		BooleanSchemaValidator
	Purpose:		Validates schemas with boolean type.

***************************************************************************************/

namespace Manatee.Json.Schema.Validators
{
	internal class BooleanSchemaValidator : IJsonSchemaValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return Equals(schema.Type, JsonSchemaTypeDefinition.Boolean);
		}

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root = null)
		{
			// This is a special case in the validator logic that doesn't need the other pattern.
			// Here we only have to check for boolean: there are no other properties.
			return json.Type != JsonValueType.Boolean
				? new SchemaValidationResults(string.Empty, $"Expected: Boolean; Actual: {json.Type}.")
				: new SchemaValidationResults();
		}
	}
}