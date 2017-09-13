namespace Manatee.Json.Schema.Validators
{
	internal class MinimumSchema04PropertyValidator : IJsonSchemaPropertyValidator<JsonSchema04>
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
	}
	internal class MinimumSchema06PropertyValidator : IJsonSchemaPropertyValidator<JsonSchema06>
	{
		public bool Applies(JsonSchema06 schema, JsonValue json)
		{
			return (schema.Minimum.HasValue || schema.ExclusiveMinimum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
		{
			var min = schema.ExclusiveMinimum ?? schema.Minimum;
			var operation = schema.ExclusiveMaximum.HasValue ? ">" : ">=";
			
			if (json.Number < min || (schema.ExclusiveMinimum.HasValue && json.Number <= min))
				return new SchemaValidationResults(string.Empty, $"Expected: {operation} {min}; Actual: {json.Number}.");

			return new SchemaValidationResults();
		}
	}
}
