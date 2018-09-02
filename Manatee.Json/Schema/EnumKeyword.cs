using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class EnumKeyword : List<JsonValue>, JsonSchemaKeyword
	{
		public virtual string Name => "enum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public EnumKeyword() { }
		public EnumKeyword(IEnumerable<JsonValue> values)
			: base(values) { }

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return Contains(json)
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, SchemaErrorMessages.Enum.ResolveTokens(new Dictionary<string, object>
					       {
						       ["value"] = json
					       }));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonArray(this);
		}
	}
}