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
 
	File Name:		MinItemsSchemaPropertyValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		MinItemsSchemaPropertyValidator
	Purpose:		Validates schema with a "minItems" property.

***************************************************************************************/
namespace Manatee.Json.Schema.Validators
{
	internal class MinItemsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.MinItems.HasValue && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Array.Count < schema.MinItems)
				return new SchemaValidationResults(string.Empty, $"Expected: >= {schema.MinItems} items; Actual: {json.Array.Count} items.");
			return new SchemaValidationResults();
		}
	}
}
