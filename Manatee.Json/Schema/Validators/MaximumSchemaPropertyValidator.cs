namespace Manatee.Json.Schema.Validators
{
	internal class MaximumSchema04PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema04 typed && (typed.Maximum.HasValue || (typed.ExclusiveMaximum ?? false)) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema04) schema;
			if (typed.ExclusiveMaximum ?? false)
			{
				if (json.Number >= typed.Maximum)
					return new SchemaValidationResults(string.Empty, $"Expected: < {typed.Maximum}; Actual: {json.Number}.");
			}
			else
			{
				if (json.Number > typed.Maximum)
					return new SchemaValidationResults(string.Empty, $"Expected: <= {typed.Maximum}; Actual: {json.Number}.");
			}
			return new SchemaValidationResults();
		}
	}
	
	// TODO: extract a base class for 6/7 
	internal class MaximumSchema06PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema06 typed && (typed.Maximum.HasValue || typed.ExclusiveMaximum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema06) schema;
			var max = typed.ExclusiveMaximum ?? typed.Maximum;
			var operation = typed.ExclusiveMaximum.HasValue ? "<" : "<=";
			
			if (json.Number > max || typed.ExclusiveMaximum.HasValue && json.Number >= max)
				return new SchemaValidationResults(string.Empty, $"Expected: {operation} {max}; Actual: {json.Number}.");
			
			return new SchemaValidationResults();
		}
	}
	
	internal class MaximumSchema07PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema06 typed && (typed.Maximum.HasValue || typed.ExclusiveMaximum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema07) schema;
			var max = typed.ExclusiveMaximum ?? typed.Maximum;
			var operation = typed.ExclusiveMaximum.HasValue ? "<" : "<=";
			
			if (json.Number > max || typed.ExclusiveMaximum.HasValue && json.Number >= max)
				return new SchemaValidationResults(string.Empty, $"Expected: {operation} {max}; Actual: {json.Number}.");
			
			return new SchemaValidationResults();
		}
	}
}
