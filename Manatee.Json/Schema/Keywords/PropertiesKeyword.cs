using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class PropertiesKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword
	{
		public string Name => "properties";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var obj = context.Instance.Object;
			var errors = new List<SchemaValidationError>();
			foreach (var property in this)
			{
				if (!obj.ContainsKey(property.Key)) continue;

				context.EvaluatedPropertyNames.Add(property.Key);
				var newContext = new SchemaValidationContext
					{
						Instance = obj[property.Key],
						Root = context.Root
					};
				var result = property.Value.Validate(newContext);
				if (result != null && !result.IsValid)
					errors.AddRange(result.Errors.Select(e => e.PrependPropertySegment(property.Key)));
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
			                         kvp => serializer.Serialize(kvp.Value))
			           .ToJson();
		}
	}
}