using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class DefinitionsKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword, IResolvePointers
	{
		public string Name => "definitions";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return SchemaValidationResults.Valid;
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
		JsonSchema IResolvePointers.Resolve(string property)
		{
			return TryGetValue(property, out var schema) ? schema : null;
		}
	}
}