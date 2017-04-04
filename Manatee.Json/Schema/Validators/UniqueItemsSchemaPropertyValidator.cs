using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class UniqueItemsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return (schema.UniqueItems ?? false) && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Array.Count != json.Array.Distinct().Count())
				return new SchemaValidationResults(string.Empty, "Expected unique items; Duplicates were found.");
			return new SchemaValidationResults();
		}
	}
}
