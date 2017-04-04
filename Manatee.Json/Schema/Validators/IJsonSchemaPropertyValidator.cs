namespace Manatee.Json.Schema.Validators
{
	internal interface IJsonSchemaPropertyValidator
	{
		bool Applies(JsonSchema schema, JsonValue json);
		SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root);
	}
}
