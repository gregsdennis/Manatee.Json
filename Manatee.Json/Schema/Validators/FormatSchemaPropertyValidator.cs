namespace Manatee.Json.Schema.Validators
{
	internal class FormatSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.Format != null && JsonSchemaOptions.ValidateFormat &&
			       json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (!schema.Format.Validate(json.String))
				return new SchemaValidationResults(string.Empty, $"Value [{json.String}] is not in an acceptable {schema.Format.Key} format.");
			return new SchemaValidationResults();
		}
	}
}
