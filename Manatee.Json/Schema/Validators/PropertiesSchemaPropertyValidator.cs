using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class PropertiesSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : JsonSchema
	{
		protected abstract IDictionary<string, JsonSchema> GetProperties(T schema);
		protected abstract IEnumerable<string> GetRequiredProperties(T schema);
		protected abstract AdditionalProperties GetAdditionalProperties(T schema);
		protected abstract Dictionary<Regex, JsonSchema> GetPatternProperties(T schema);
		protected virtual IEnumerable<SchemaValidationError> AdditionValidation(JsonSchema schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationError[] { };
		}
		
		public virtual bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema is T typed &&
			       (GetProperties(typed) != null ||
			        GetAdditionalProperties(typed) != null ||
			        GetPatternProperties(typed) != null ||
			        GetRequiredProperties(typed) != null) &&
			       json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(JsonSchema typed, JsonValue json, JsonValue root)
		{
			var obj = json.Object;
			var errors = new List<SchemaValidationError>();
			var properties = typed.Properties() ?? new Dictionary<string, JsonSchema>();
			var additionalProperties = typed.AdditionalProperties();
			var patternProperties = typed.PatternProperties();
			foreach (var property in properties)
			{
				if (!obj.ContainsKey(property.Key)) continue;
				var result = property.Value?.Validate(obj[property.Key], null);
				if (result != null && !result.IsValid)
					errors.AddRange(result.Errors.Select(e => e.PrependPropertySegment(property.Key)));
			}
			var requiredProperties = typed.Required() ?? new List<string>();
			foreach (var property in requiredProperties)
			{
				if (!obj.ContainsKey(property))
				{
					var message = SchemaErrorMessages.Required.ResolveTokens(new Dictionary<string, object>
						{
							["property"] = property,
							["value"] = json
					});
					errors.Add(new SchemaValidationError(property, message));
				}
			}
			// if additionalProperties is false, we perform the property elimination,
			// otherwise properties and patternProperties applies to all properties.
			var extraData = Equals(additionalProperties, JsonSchema.False)
				                ? obj.Where(kvp => properties.All(p => p.Key != kvp.Key))
				                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
				                : obj;
			if (patternProperties != null)
			{
				var propertiesToRemove = new List<string>();
				foreach (var patternProperty in patternProperties)
				{
					var pattern = new Regex(patternProperty.Key);
					var localSchema = patternProperty.Value;
					var matches = extraData.Keys.Where(k => pattern.IsMatch(k));
					foreach (var match in matches)
					{
						var matchErrors = localSchema.Validate(extraData[match], null).Errors;
						errors.AddRange(matchErrors.Select(e => new SchemaValidationError(match, e.Message)));
					}
					propertiesToRemove.AddRange(extraData.Keys.Where(k => pattern.IsMatch(k)));
				}
				extraData = extraData.Where(kvp => !propertiesToRemove.Contains(kvp.Key))
				                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			}
			// additionalProperties never applies to properties checked by
			// properties or patternProperties.
			extraData = extraData.Where(kvp => properties.All(p => p.Key != kvp.Key))
			                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			if (additionalProperties != null)
			{
				if (Equals(additionalProperties, JsonSchema.False) && extraData.Any())
					errors.AddRange(extraData.Keys.Select(k =>
						{
							var message = SchemaErrorMessages.AdditionalProperties_False.ResolveTokens(new Dictionary<string, object>
								{
									["property"] = k,
									["value"] = json
							});
							return new SchemaValidationError(k, message);
						}));
				else
				{
					var localSchema = additionalProperties;
					foreach (var key in extraData.Keys)
					{
						var extraErrors = localSchema.Validate(extraData[key], null).Errors;
						errors.AddRange(extraErrors.Select(e => e.PrependPropertySegment(key)));
					}
				}
			}
			var additionalValidation = AdditionValidation(typed, json, root);
			return new SchemaValidationResults(errors.Concat(additionalValidation));
		}
	}
}
