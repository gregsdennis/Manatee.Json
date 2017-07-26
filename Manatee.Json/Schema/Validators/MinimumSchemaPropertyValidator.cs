namespace Manatee.Json.Schema.Validators
{
	internal class MinimumSchemaPropertyValidator : IJsonSchemaPropertyValidator<JsonSchema04>,
	                                                IJsonSchemaPropertyValidator<JsonSchema06>
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return (schema.Minimum.HasValue || (schema.ExclusiveMinimum ?? false)) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
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

		public bool Applies(JsonSchema06 schema, JsonValue json)
		{
			return (schema.Minimum.HasValue || schema.ExclusiveMinimum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
		{
			if (json.Number <= schema.ExclusiveMinimum)
				return new SchemaValidationResults(string.Empty, $"Expected: > {schema.ExclusiveMinimum}; Actual: {json.Number}.");
			if (json.Number < schema.Minimum)
				return new SchemaValidationResults(string.Empty, $"Expected: >= {schema.Minimum}; Actual: {json.Number}.");
			return new SchemaValidationResults();
		}
	}
}
