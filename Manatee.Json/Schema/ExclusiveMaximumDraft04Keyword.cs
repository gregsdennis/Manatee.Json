using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ExclusiveMaximumDraft04Keyword : JsonSchemaKeyword
	{
		public string Name => "exclusiveMaximum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;

		public bool Value { get; private set; }

		public ExclusiveMaximumDraft04Keyword(bool value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Boolean;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
	}
}