namespace Manatee.Json.Schema.Validators
{
	internal class MinPropertiesSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return schema.MinProperties.HasValue && json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			return json.Object.Count < schema.MinProperties
				       ? new SchemaValidationResults(string.Empty, $"Expected: >= {schema.MinProperties} items; Actual: {json.Object.Count} items.")
				       : new SchemaValidationResults();
		}
	}
}
