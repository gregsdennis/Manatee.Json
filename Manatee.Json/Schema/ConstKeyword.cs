using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ConstKeyword : IJsonSchemaKeyword
	{
		public string Name => "const";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public JsonValue Value { get; private set; }

		public ConstKeyword(JsonValue value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
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