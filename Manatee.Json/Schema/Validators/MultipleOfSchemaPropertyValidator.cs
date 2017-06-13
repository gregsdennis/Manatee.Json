namespace Manatee.Json.Schema.Validators
{
	internal class MultipleOfSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.MultipleOf.HasValue && json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if ((decimal) json.Number%(decimal) schema.MultipleOf.Value != 0)
				return new SchemaValidationResults(string.Empty, $"Expected: {json.Number}%{schema.MultipleOf}=0; Actual: {json.Number%schema.MultipleOf}.");
			return new SchemaValidationResults();
		}
	}
}
