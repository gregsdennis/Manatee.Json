using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class PatternPropertiesKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword
	{
		public string Name => "patternProperties";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var obj = context.Instance.Object;
			var properties = context.Local.Properties() ?? new Dictionary<string, JsonSchema>();
			var additionalProperties = context.Local.AdditionalProperties();

			// if additionalProperties is false, we perform the property elimination,
			// otherwise properties and patternProperties applies to all properties.
			var extraData = Equals(additionalProperties, JsonSchema.False)
				? obj.Where(kvp => properties.All(p => p.Key != kvp.Key))
					.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
				: obj;
			var propertiesToRemove = new List<string>();
			foreach (var patternProperty in this)
			{
				var pattern = new Regex(patternProperty.Key);
				var localSchema = patternProperty.Value;
				var matches = extraData.Keys.Where(k => pattern.IsMatch(k));
				foreach (var match in matches)
				{
					var newContext = new SchemaValidationContext
						{
							Instance = extraData[match],
							Root = context.Root
					};
					var result = localSchema.Validate(newContext);
					context.EvaluatedPropertyNames.Add(match);
					context.EvaluatedPropertyNames.AddRange(newContext.EvaluatedPropertyNames);
					errors.AddRange(result.Errors.Select(e => new SchemaValidationError(match, e.Message)));
				}
				propertiesToRemove.AddRange(extraData.Keys.Where(k => pattern.IsMatch(k)));
			}

			return new SchemaValidationResults(errors);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			foreach (var kvp in json.Object)
			{
				this[kvp.Key] = serializer.Deserialize<JsonSchema>(kvp.Value);
			}
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.ToDictionary(kvp => kvp.Key,
			                         kvp => serializer.Serialize<JsonSchema>(kvp.Value))
				.ToJson();
		}
	}
}