namespace Manatee.Json.Schema.Validators
{
	internal abstract class MinItemsSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract uint? GetMinItems(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMinItems(typed).HasValue && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var minItems = GetMinItems((T) schema);
			return json.Array.Count < minItems
				       ? new SchemaValidationResults(string.Empty, $"Expected: >= {minItems} items; Actual: {json.Array.Count} items.")
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
	
	internal class MinItemsSchema07PropertyValidator : MinItemsSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override uint? GetMinItems(JsonSchema07 schema)
		{
			return schema.MinItems;
		}
	}
}
