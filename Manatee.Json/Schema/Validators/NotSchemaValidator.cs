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
 
	File Name:		NotSchemaValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		NotSchemaValidator
	Purpose:		Validates schemas with a "not" property.

***************************************************************************************/
namespace Manatee.Json.Schema.Validators
{
	internal class NotSchemaValidator : IJsonSchemaValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return schema.Not != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var results = schema.Not.Validate(json, root);
			if (results.Valid)
				return new SchemaValidationResults(string.Empty, "Expected schema to be invalid, but was valid.");
			return new SchemaValidationResults();
		}
	}
}
