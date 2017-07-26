using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class UniqueItemsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return (schema.UniqueItems ?? false) && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			return json.Array.Count != json.Array.Distinct().Count()
				       ? new SchemaValidationResults(string.Empty, "Expected unique items; Duplicates were found.")
				       : new SchemaValidationResults();
		}
	}
}
