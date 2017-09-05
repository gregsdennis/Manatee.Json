namespace Manatee.Json.Schema.Validators
{
	internal abstract class NotSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract IJsonSchema GetNot(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetNot(schema) != null;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var results = GetNot(schema).Validate(json, root);
			return results.Valid
				       ? new SchemaValidationResults(string.Empty, "Expected schema to be invalid, but was valid.")
				       : new SchemaValidationResults();
		}
	}
	
	internal class NotSchema04PropertyValidator : NotSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IJsonSchema GetNot(JsonSchema04 schema)
		{
			return schema.Not;
		}
	}
	
	internal class NotSchema06PropertyValidator : NotSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IJsonSchema GetNot(JsonSchema06 schema)
		{
			return schema.Not;
		}
	}
}
