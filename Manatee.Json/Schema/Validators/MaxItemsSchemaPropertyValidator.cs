namespace Manatee.Json.Schema.Validators
{
	internal class MaxItemsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return _GetMaxItems(schema).HasValue && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var maxItems = _GetMaxItems(schema);
			if (json.Array.Count > maxItems)
				return new SchemaValidationResults(string.Empty, $"Expected: <= {maxItems} items; Actual: {json.Array.Count} items.");
			return new SchemaValidationResults();
		}

		private static uint? _GetMaxItems(IJsonSchema schema)
		{
			return (schema as JsonSchema04)?.MaxItems ?? (schema as JsonSchema06)?.MaxItems;
		}
	}
}
