namespace Manatee.Json.Schema
{
	public interface IJsonSchemaPropertyValidator
	{
		bool Applies(IJsonSchema schema, JsonValue json);
		SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root);
	}
}
