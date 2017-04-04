using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal class MaxLengthSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.MaxLength.HasValue && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var length = new StringInfo(json.String).LengthInTextElements;
			if (schema.MaxLength.HasValue && (length > schema.MaxLength))
				return new SchemaValidationResults(string.Empty, $"Expected: length <= {schema.MaxLength}; Actual: {length}.");
			return new SchemaValidationResults();
		}
	}
}
