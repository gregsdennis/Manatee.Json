namespace Manatee.Json.Schema.Validators
{
	internal class MinItemsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.MinItems.HasValue && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			return json.Array.Count < schema.MinItems
				       ? new SchemaValidationResults(string.Empty, $"Expected: >= {schema.MinItems} items; Actual: {json.Array.Count} items.")
				       : new SchemaValidationResults();
		}
	}
}
