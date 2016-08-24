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
 
	File Name:		PropertiesSchemaValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		PropertiesSchemaValidator
	Purpose:		Validates schemas with "properties", "additionalProperties",
					or "patternProperties" properties.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class PropertiesSchemaValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return schema.Properties != null ||
				   schema.AdditionalProperties != null ||
				   schema.PatternProperties != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Type != JsonValueType.Object) return new SchemaValidationResults();
			var obj = json.Object;
			var errors = new List<SchemaValidationError>();
			var properties = schema.Properties ?? new JsonSchemaPropertyDefinitionCollection();
			foreach (var property in properties)
			{
				if (!obj.ContainsKey(property.Name))
				{
					if (property.IsRequired)
						errors.Add(new SchemaValidationError(property.Name, "Required property not found."));
					continue;
				}
				var result = property.Type.Validate(obj[property.Name], root);
				if (!result.Valid)
					errors.AddRange(result.Errors.Select(e => e.PrependPropertyName(property.Name)));
			}
			var extraData = obj.Where(kvp => properties.All(p => p.Name != kvp.Key))
							   .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			// TODO: validate this process
			if (schema.AdditionalProperties != null && schema.AdditionalProperties.Equals(AdditionalProperties.False))
			{
				if (schema.PatternProperties != null)
				{
					foreach (var patternProperty in schema.PatternProperties)
					{
						var pattern = patternProperty.Key;
						var localSchema = patternProperty.Value;
						var matches = extraData.Keys.Where(k => pattern.IsMatch(k));
						foreach (var match in matches)
						{
							var matchErrors = localSchema.Validate(extraData[match], root).Errors;
							errors.AddRange(matchErrors.Select(e => new SchemaValidationError(match, e.Message)));
						}
						extraData = extraData.Where(kvp => !pattern.IsMatch(kvp.Key))
											 .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
					}
				}
				errors.AddRange(extraData.Keys.Select(k => new SchemaValidationError(k, "Cannot find a match within Properties or PatternProperties.")));
			}
			else if (schema.AdditionalProperties != null)
			{
				var localSchema = schema.AdditionalProperties.Definition;
				foreach (var key in extraData.Keys)
				{
					var extraErrors = localSchema.Validate(extraData[key], root).Errors;
					errors.AddRange(extraErrors.Select(e => e.PrependPropertyName(key)));
				}
			}
			return new SchemaValidationResults(errors);
		}
	}
}
