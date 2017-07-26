using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal class MinLengthSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return schema.MinLength.HasValue && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			var length = new StringInfo(json.String).LengthInTextElements;
			if (schema.MinLength.HasValue && length < schema.MinLength)
				return new SchemaValidationResults(string.Empty, $"Expected: length >= {schema.MinLength}; Actual: {length}.");
			return new SchemaValidationResults();
		}
	}
}
