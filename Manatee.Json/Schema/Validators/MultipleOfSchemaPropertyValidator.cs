namespace Manatee.Json.Schema.Validators
{
	internal abstract class MultipleOfSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract double? GetMultipleOf(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetMultipleOf(schema).HasValue && json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			return (decimal) json.Number % (decimal?) GetMultipleOf(schema) != 0
				       ? new SchemaValidationResults(string.Empty, $"Expected: {json.Number}%{GetMultipleOf(schema)}=0; Actual: {json.Number % GetMultipleOf(schema)}.")
				       : new SchemaValidationResults();
		}
	}
	
	internal class MultipleOfSchema04PropertyValidator : MultipleOfSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override double? GetMultipleOf(JsonSchema04 schema)
		{
			return schema.MultipleOf;
		}
	}
	
	internal class MultipleOfSchema06PropertyValidator : MultipleOfSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override double? GetMultipleOf(JsonSchema06 schema)
		{
			return schema.MultipleOf;
		}
	}
}
