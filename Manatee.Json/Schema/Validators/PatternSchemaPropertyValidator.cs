using System.Text.RegularExpressions;

namespace Manatee.Json.Schema.Validators
{
	internal class PatternSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.Pattern != null && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (!Regex.IsMatch(json.String, schema.Pattern))
				return new SchemaValidationResults(string.Empty, $"Value [{json.String}] does not match required Regex pattern [{schema.Pattern}].");
			return new SchemaValidationResults();
		}
	}
}
