namespace Manatee.Json.Schema.Validators
{
	internal abstract class MinPropertiesSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract uint? GetMinProperties(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetMinProperties(schema).HasValue && json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			return json.Object.Count < GetMinProperties(schema)
				       ? new SchemaValidationResults(string.Empty, $"Expected: >= {GetMinProperties(schema)} items; Actual: {json.Object.Count} items.")
				       : new SchemaValidationResults();
		}
	}
	
	internal class MinPropertiesSchema04PropertyValidator : MinPropertiesSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMinProperties(JsonSchema04 schema)
		{
			return schema.MinProperties;
		}
	}
	
	internal class MinPropertiesSchema06PropertyValidator : MinPropertiesSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMinProperties(JsonSchema06 schema)
		{
			return schema.MinProperties;
		}
	}
}
