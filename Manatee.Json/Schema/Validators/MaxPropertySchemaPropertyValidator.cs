namespace Manatee.Json.Schema.Validators
{
	internal class MaxPropertySchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.MaxProperties.HasValue && json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Object.Count > schema.MaxProperties)
				return new SchemaValidationResults(string.Empty, $"Expected: <= {schema.MaxProperties} properties; Actual: {json.Object.Count} properties.");
			return new SchemaValidationResults();
		}
	}
}
