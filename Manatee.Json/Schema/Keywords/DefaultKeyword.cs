using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class DefaultKeyword : IJsonSchemaKeyword
	{
		public virtual string Name => "default";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public JsonValue Value { get; private set; }

		public DefaultKeyword(JsonValue value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return SchemaValidationResults.Valid;
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