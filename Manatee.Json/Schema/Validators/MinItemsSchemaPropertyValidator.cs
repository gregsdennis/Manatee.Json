namespace Manatee.Json.Schema.Validators
{
	internal abstract class MinItemsSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract uint? GetMinItems(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetMinItems(schema).HasValue && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			return json.Array.Count < GetMinItems(schema)
				       ? new SchemaValidationResults(string.Empty, $"Expected: >= {GetMinItems(schema)} items; Actual: {json.Array.Count} items.")
				       : new SchemaValidationResults();
		}
	}
	
	internal class MinItemsSchema04PropertyValidator : MinItemsSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMinItems(JsonSchema04 schema)
		{
			return schema.MinItems;
		}
	}
	
	internal class MinItemsSchema06PropertyValidator : MinItemsSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMinItems(JsonSchema06 schema)
		{
			return schema.MinItems;
		}
	}
}
