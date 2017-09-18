using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class PropertiesSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract JsonSchemaPropertyDefinitionCollection GetProperties(T schema);
		protected abstract AdditionalProperties GetAdditionalProperties(T schema);
		protected abstract Dictionary<Regex, IJsonSchema> GetPatternProperties(T schema);
		protected virtual IEnumerable<SchemaValidationError> AdditionValidation(T schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationError[] { };
		}
		
		public virtual bool Applies(T schema, JsonValue json)
		{
			return (GetProperties(schema) != null ||
			        GetAdditionalProperties(schema) != null ||
			        GetPatternProperties(schema) != null) &&
					json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var obj = json.Object;
			var errors = new List<SchemaValidationError>();
			var properties = GetProperties(schema) ?? new JsonSchemaPropertyDefinitionCollection();
			foreach (var property in properties)
			{
				if (!obj.ContainsKey(property.Name))
				{
					if (property.IsRequired)
						errors.Add(new SchemaValidationError(property.Name, "Required property not found."));
					continue;
				}
				var result = property.Type?.Validate(obj[property.Name], root);
				if (result != null && !result.Valid)
					errors.AddRange(result.Errors.Select(e => e.PrependPropertyName(property.Name)));
			}
			// if additionalProperties is false, we perform the property elimination,
			// otherwise properties and patternProperties applies to all properties.
			var extraData = Equals(GetAdditionalProperties(schema), AdditionalProperties.False)
				                ? obj.Where(kvp => properties.All(p => p.Name != kvp.Key))
				                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
				                : obj;
			if (GetPatternProperties(schema) != null)
			{
				var propertiesToRemove = new List<string>();
				foreach (var patternProperty in GetPatternProperties(schema))
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
			if (GetAdditionalProperties(schema) != null)
			{
				if (Equals(GetAdditionalProperties(schema), AdditionalProperties.False) && extraData.Any())
					errors.AddRange(extraData.Keys.Select(k => new SchemaValidationError(k, "Additional properties are not allowed.")));
				else
				{
					var localSchema = GetAdditionalProperties(schema).Definition;
					foreach (var key in extraData.Keys)
					{
						var extraErrors = localSchema.Validate(extraData[key], root).Errors;
						errors.AddRange(extraErrors.Select(e => e.PrependPropertyName(key)));
					}
				}
			}
			var additionalValidation = AdditionValidation(schema, json, root);
			return new SchemaValidationResults(errors.Concat(additionalValidation));
		}
	}

	internal class PropertiesSchema04PropertyValidator : PropertiesSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override JsonSchemaPropertyDefinitionCollection GetProperties(JsonSchema04 schema)
		{
			return schema.Properties;
		}
		protected override AdditionalProperties GetAdditionalProperties(JsonSchema04 schema)
		{
			return schema.AdditionalProperties;
		}
		protected override Dictionary<Regex, IJsonSchema> GetPatternProperties(JsonSchema04 schema)
		{
			return schema.PatternProperties;
		}
	}
	
	internal class PropertiesSchema06PropertyValidator : PropertiesSchemaPropertyValidatorBase<JsonSchema06>
	{
		public override bool Applies(JsonSchema06 schema, JsonValue json)
		{
			return base.Applies(schema, json) || (json.Type == JsonValueType.Object && schema.PropertyNames != null);
		}

		protected override JsonSchemaPropertyDefinitionCollection GetProperties(JsonSchema06 schema)
		{
			return schema.Properties;
		}
		protected override AdditionalProperties GetAdditionalProperties(JsonSchema06 schema)
		{
			if (schema.AdditionalProperties == null) return null;
			return new AdditionalProperties {Definition = schema.AdditionalProperties};
		}
		protected override Dictionary<Regex, IJsonSchema> GetPatternProperties(JsonSchema06 schema)
		{
			return schema.PatternProperties;
		}
		protected override IEnumerable<SchemaValidationError> AdditionValidation(JsonSchema06 schema, JsonValue json, JsonValue root)
		{
			return schema.PropertyNames == null
				       ? new SchemaValidationError[] { }
				       : json.Object.Keys.SelectMany(k => schema.PropertyNames.Validate(k).Errors);
		}
	}
}
