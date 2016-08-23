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
 
	File Name:		NullSchemaValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		NullSchemaValidator
	Purpose:		Validates schemas with null type.

***************************************************************************************/
namespace Manatee.Json.Schema.Validators
{
	internal class NullSchemaValidator : IJsonSchemaValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return Equals(schema.Type, JsonSchemaTypeDefinition.Null);
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			// This is a special case in the validator logic that doesn't need the other pattern.
			// Here we only have to check for null: there are no other properties.
			return json.Type != JsonValueType.Null
				? new SchemaValidationResults(string.Empty, $"Expected: Null; Actual: {json.Type}.")
				: new SchemaValidationResults();
		}
	}
}
