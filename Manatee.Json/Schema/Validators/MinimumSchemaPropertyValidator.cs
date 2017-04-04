namespace Manatee.Json.Schema.Validators
{
	internal class MinimumSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return (schema.Minimum.HasValue || (schema.ExclusiveMinimum ?? false)) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (schema.ExclusiveMinimum ?? false)
			{
				if (json.Number <= schema.Minimum)
					return new SchemaValidationResults(string.Empty, $"Expected: > {schema.Minimum}; Actual: {json.Number}.");
			}
			else
			{
				if (json.Number < schema.Minimum)
					return new SchemaValidationResults(string.Empty, $"Expected: >= {schema.Minimum}; Actual: {json.Number}.");
			}
			return new SchemaValidationResults();
		}
	}
}
