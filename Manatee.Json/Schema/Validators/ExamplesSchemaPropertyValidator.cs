namespace Manatee.Json.Schema.Validators
{
	internal class ExamplesSchemaPropertyValidator : IJsonSchemaPropertyValidator<JsonSchema06>
	{
		public bool Applies(JsonSchema06 schema, JsonValue json)
		{
			return schema.Examples != null;
		}
		public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
		{
			if (json.Type != JsonValueType.Array)
				return new SchemaValidationResults(string.Empty, $"Expected: Array; Actual: {json.Type}");
			return new SchemaValidationResults();
		}
	}
}