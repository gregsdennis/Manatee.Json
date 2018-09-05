using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class AllOfKeyword : List<JsonSchema>, IJsonSchemaKeyword
	{
		public virtual string Name => "allOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return new SchemaValidationResults(this.Select(s =>
				{
					var newContext = new SchemaValidationContext
						{
							Instance = context.Instance,
							Root = context.Root
						};
					var result = s.Validate(newContext);
					context.EvaluatedPropertyNames.AddRange(newContext.EvaluatedPropertyNames);
					return result;
				}));
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