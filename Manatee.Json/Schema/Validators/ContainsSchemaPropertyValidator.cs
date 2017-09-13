using System.Linq;

namespace Manatee.Json.Schema.Validators
{
    internal class ContainsSchemaPropertyValidator : IJsonSchemaPropertyValidator<JsonSchema06>
    {
        public bool Applies(JsonSchema06 schema, JsonValue json)
        {
            return json.Type == JsonValueType.Array && schema.Contains != null;
        }
        public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
        {
            if (json.Array.Count == 0)
                return new SchemaValidationResults(string.Empty, $"Expected an item that matched '{schema.Contains}' but no items were found.");
            
            var validations = json.Array.Select(jv => schema.Contains.Validate(jv, root)).ToList();
            if (!validations.Any(v => v.Valid))
                return new SchemaValidationResults(validations);
            
            return new SchemaValidationResults();
        }
    }
}