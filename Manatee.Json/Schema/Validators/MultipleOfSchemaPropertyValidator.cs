namespace Manatee.Json.Schema.Validators
{
	internal abstract class MultipleOfSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract double? GetMultipleOf(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMultipleOf(typed).HasValue && json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var multipleOf = GetMultipleOf((T) schema);
			return (decimal) json.Number % (decimal?) multipleOf != 0
				       ? new SchemaValidationResults(string.Empty, $"Expected: {json.Number}%{multipleOf}=0; Actual: {json.Number % multipleOf}.")
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
	
	internal class MultipleOfSchema07PropertyValidator : MultipleOfSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override double? GetMultipleOf(JsonSchema07 schema)
		{
			return schema.MultipleOf;
		}
	}
}
