namespace Manatee.Json.Schema.Validators
{
	internal class NotSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.Not != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var results = schema.Not.Validate(json, root);
			return results.Valid
				       ? new SchemaValidationResults(string.Empty, "Expected schema to be invalid, but was valid.")
				       : new SchemaValidationResults();
		}
	}
}
