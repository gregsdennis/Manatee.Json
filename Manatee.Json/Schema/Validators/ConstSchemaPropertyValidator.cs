namespace Manatee.Json.Schema.Validators
{
    internal class ConstSchemaPropertyValidator : IJsonSchemaPropertyValidator<JsonSchema06>
    {
        public bool Applies(JsonSchema06 schema, JsonValue json)
        {
            return schema.Const != null;
        }
        public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
        {
            if (schema.Const != json)
                return new SchemaValidationResults(string.Empty, $"Expected: {schema.Const}; Actual: {json}.");
            return new SchemaValidationResults();
        }
    }
}