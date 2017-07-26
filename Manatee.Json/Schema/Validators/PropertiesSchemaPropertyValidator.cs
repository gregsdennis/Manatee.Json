using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class PropertiesSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return (schema.Properties != null ||
			        schema.AdditionalProperties != null ||
			        schema.PatternProperties != null) &&
					json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
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
			// if additionalProperties is false, we perform the property elimination,
			// otherwise properties and patternProperties applies to all properties.
			var extraData = Equals(schema.AdditionalProperties, AdditionalProperties.False)
				                ? obj.Where(kvp => properties.All(p => p.Name != kvp.Key))
				                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
				                : obj;
			if (schema.PatternProperties != null)
			{
				var propertiesToRemove = new List<string>();
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
					propertiesToRemove.AddRange(extraData.Keys.Where(k => pattern.IsMatch(k)));
				}
				extraData = extraData.Where(kvp => !propertiesToRemove.Contains(kvp.Key))
				                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			}
			// additionalProperties never applies to properties checked by
			// properties or patternProperties.
			extraData = extraData.Where(kvp => properties.All(p => p.Name != kvp.Key))
			                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			if (schema.AdditionalProperties != null)
			{
				if (Equals(schema.AdditionalProperties, AdditionalProperties.False) && extraData.Any())
					errors.AddRange(extraData.Keys.Select(k => new SchemaValidationError(k, "Additional properties are not allowed.")));
				else
				{
					var localSchema = schema.AdditionalProperties.Definition;
					foreach (var key in extraData.Keys)
					{
						var extraErrors = localSchema.Validate(extraData[key], root).Errors;
						errors.AddRange(extraErrors.Select(e => e.PrependPropertyName(key)));
					}
				}
			}
			return new SchemaValidationResults(errors);
		}
	}
}
