namespace Manatee.Json.Schema.Validators
{
	internal class MaximumSchema04PropertyValidator : IJsonSchemaPropertyValidator<JsonSchema04>
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
	
	internal class MaximumSchema06PropertyValidator : IJsonSchemaPropertyValidator<JsonSchema06>
	{
		public bool Applies(JsonSchema06 schema, JsonValue json)
		{
			return (schema.Maximum.HasValue || schema.ExclusiveMaximum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
		{
			var max = schema.ExclusiveMaximum ?? schema.Maximum;
			var operation = schema.ExclusiveMaximum.HasValue ? "<" : "<=";
			
			if (json.Number >= max)
				return new SchemaValidationResults(string.Empty, $"Expected: {operation} {max}; Actual: {json.Number}.");
			
			return new SchemaValidationResults();
		}
	}
}
