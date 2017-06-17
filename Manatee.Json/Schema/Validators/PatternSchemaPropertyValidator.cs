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
			return Regex.IsMatch(json.String, schema.Pattern)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(string.Empty, $"Value [{json.String}] does not match required Regex pattern [{schema.Pattern}].");
		}
	}
}
