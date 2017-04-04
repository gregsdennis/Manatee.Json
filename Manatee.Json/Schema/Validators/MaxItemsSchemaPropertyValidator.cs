namespace Manatee.Json.Schema.Validators
{
	internal class MaxItemsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.MaxItems.HasValue && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Array.Count > schema.MaxItems)
				return new SchemaValidationResults(string.Empty, $"Expected: <= {schema.MaxItems} items; Actual: {json.Array.Count} items.");
			return new SchemaValidationResults();
		}
	}
}
