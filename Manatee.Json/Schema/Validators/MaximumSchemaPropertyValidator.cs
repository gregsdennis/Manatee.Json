namespace Manatee.Json.Schema.Validators
{
	internal class MaximumSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return (schema.Maximum.HasValue || (schema.ExclusiveMaximum ?? false)) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			if (schema.ExclusiveMaximum ?? false)
			{
				if (json.Number >= schema.Maximum)
					return new SchemaValidationResults(string.Empty, $"Expected: < {schema.Maximum}; Actual: {json.Number}.");
			}
			else
			{
				if (json.Number > schema.Maximum)
					return new SchemaValidationResults(string.Empty, $"Expected: <= {schema.Maximum}; Actual: {json.Number}.");
			}
			return new SchemaValidationResults();
		}
	}
}
