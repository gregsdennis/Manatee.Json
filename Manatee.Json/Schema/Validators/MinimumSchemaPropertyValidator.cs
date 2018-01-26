namespace Manatee.Json.Schema.Validators
{
	internal class MinimumSchema04PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema04 typed && (typed.Minimum.HasValue || (typed.ExclusiveMinimum ?? false)) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema04)schema;
			if (typed.ExclusiveMinimum ?? false)
			{
				if (json.Number <= typed.Minimum)
					return new SchemaValidationResults(string.Empty, $"Expected: > {typed.Minimum}; Actual: {json.Number}.");
			}
			else
			{
				if (json.Number < typed.Minimum)
					return new SchemaValidationResults(string.Empty, $"Expected: >= {typed.Minimum}; Actual: {json.Number}.");
			}
			return new SchemaValidationResults();
		}
	}

	// TODO: extract a base class for 6/7 
	internal class MinimumSchema06PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema06 typed && (typed.Minimum.HasValue || typed.ExclusiveMinimum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema06)schema;
			var min = typed.ExclusiveMinimum ?? typed.Minimum;
			var operation = typed.ExclusiveMaximum.HasValue ? ">" : ">=";
			
			if (json.Number < min || (typed.ExclusiveMinimum.HasValue && json.Number <= min))
				return new SchemaValidationResults(string.Empty, $"Expected: {operation} {min}; Actual: {json.Number}.");

			return new SchemaValidationResults();
		}
	}

	internal class MinimumSchema07PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema07 typed && (typed.Minimum.HasValue || typed.ExclusiveMinimum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema07) schema;
			var min = typed.ExclusiveMinimum ?? typed.Minimum;
			var operation = typed.ExclusiveMaximum.HasValue ? ">" : ">=";
			
			if (json.Number < min || (typed.ExclusiveMinimum.HasValue && json.Number <= min))
				return new SchemaValidationResults(string.Empty, $"Expected: {operation} {min}; Actual: {json.Number}.");

			return new SchemaValidationResults();
		}
	}
}
