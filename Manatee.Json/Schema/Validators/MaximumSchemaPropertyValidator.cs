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
 
	File Name:		MaximumSchemaPropertyValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		MaximumSchemaPropertyValidator
	Purpose:		Validates schema with "maximum" or "exclusiveMaximum" properties.

***************************************************************************************/
namespace Manatee.Json.Schema.Validators
{
	internal class MaximumSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return schema.Maximum.HasValue || (schema.ExclusiveMaximum ?? false);
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Type == JsonValueType.Number)
			{
				if (schema.ExclusiveMaximum ?? false)
				{
					if (json.Number >= schema.Maximum)
						return new SchemaValidationResults(string.Empty, $"Expected: < {schema.Maximum}; Actual: {json.Number}.");
				}
				else
				{
					if (json.Number > schema.Maximum)
						return new SchemaValidationResults(string.Empty, $"Expected: <= {schema.Maximum}; Actual: {json.Number}.");
				}
			}
			return new SchemaValidationResults();
		}
	}
}
