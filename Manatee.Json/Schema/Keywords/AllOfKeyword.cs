using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class AllOfKeyword : List<JsonSchema>, IJsonSchemaKeyword
	{
		public virtual string Name => "allOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return new SchemaValidationResults(this.Select(s => s.Validate(json, root)));
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