using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class AnyOfKeyword : List<JsonSchema>, IJsonSchemaKeyword
	{
		public virtual string Name => "anyOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = this.Select(s =>
				{
					var newContext = new SchemaValidationContext
						{
							Instance = context.Instance,
							Root = context.Root
						};
					var result = s.Validate(newContext);
					context.EvaluatedPropertyNames.AddRange(newContext.EvaluatedPropertyNames);
					return result;
				}).ToList();
			return results.Any(r => r.IsValid)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(results);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(serializer.Deserialize<JsonSchema>));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.Select(serializer.Serialize).ToJson();
		}
	}
}