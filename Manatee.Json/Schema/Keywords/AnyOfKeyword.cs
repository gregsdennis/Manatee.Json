using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class AnyOfKeyword : List<JsonSchema>, IJsonSchemaKeyword
	{
		public virtual string Name => "anyOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			var errors = this.Select(s => s.Validate(json, root)).ToList();
			return errors.Any(r => r.IsValid)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(errors);
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