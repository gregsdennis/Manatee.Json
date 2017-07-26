namespace Manatee.Json.Schema.Validators
{
	internal class MaxPropertySchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return _GetMaxProperties(schema).HasValue && json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var maxProperties = _GetMaxProperties(schema);
			return json.Object.Count > maxProperties
					   ? new SchemaValidationResults(string.Empty, $"Expected: <= {maxProperties} properties; Actual: {json.Object.Count} properties.")
				       : new SchemaValidationResults();
		}

		private static uint? _GetMaxProperties(IJsonSchema schema)
		{
			return (schema as JsonSchema04)?.MaxProperties ?? (schema as JsonSchema06)?.MaxProperties;
		}
	}
}
