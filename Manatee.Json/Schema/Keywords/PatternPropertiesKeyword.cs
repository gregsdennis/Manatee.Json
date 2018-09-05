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
		public int ValidationSequence => 2;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var obj = context.Instance.Object;

			foreach (var patternProperty in this)
			{
				var pattern = new Regex(patternProperty.Key);
				var localSchema = patternProperty.Value;
				var matches = obj.Keys.Where(k => pattern.IsMatch(k));
				foreach (var match in matches)
				{
					context.EvaluatedPropertyNames.Add(match);
					var newContext = new SchemaValidationContext
						{
							Instance = obj[match],
							Root = context.Root
					};
					var result = localSchema.Validate(newContext);
					errors.AddRange(result.Errors.Select(e => new SchemaValidationError(match, e.Message)));
				}
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