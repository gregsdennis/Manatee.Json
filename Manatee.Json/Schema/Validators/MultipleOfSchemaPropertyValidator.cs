namespace Manatee.Json.Schema.Validators
{
	internal class MultipleOfSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return schema.MultipleOf.HasValue && json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			return (decimal) json.Number % (decimal?) schema.MultipleOf != 0
				       ? new SchemaValidationResults(string.Empty, $"Expected: {json.Number}%{schema.MultipleOf}=0; Actual: {json.Number % schema.MultipleOf}.")
				       : new SchemaValidationResults();
		}
	}
}
