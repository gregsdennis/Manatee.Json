using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ConstKeyword : JsonSchemaKeyword
	{
		public string Name => "const";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public JsonValue Value { get; private set; }

		public ConstKeyword(JsonValue value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return json == Value
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, "expected the indicated value");
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
	}
}