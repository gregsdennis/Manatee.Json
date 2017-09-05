namespace Manatee.Json.Schema.Validators
{
	internal abstract class MaxItemsSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract uint? GetMaxItems(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetMaxItems(schema).HasValue && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var maxItems = GetMaxItems(schema);
			if (json.Array.Count > maxItems)
				return new SchemaValidationResults(string.Empty, $"Expected: <= {maxItems} items; Actual: {json.Array.Count} items.");
			return new SchemaValidationResults();
		}
	}
	
	internal class MaxItemsSchema04PropertyValidator : MaxItemsSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMaxItems(JsonSchema04 schema)
		{
			return schema.MaxItems;
		}
	}
	
	internal class MaxItemsSchema06PropertyValidator : MaxItemsSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMaxItems(JsonSchema06 schema)
		{
			return schema.MaxItems;
		}
	}
}
