using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal class MaxLengthSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return _GetMaxLength(schema).HasValue && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var maxLength = _GetMaxLength(schema);
			var length = new StringInfo(json.String).LengthInTextElements;
			if (maxLength.HasValue && length > maxLength)
				return new SchemaValidationResults(string.Empty, $"Expected: length <= {maxLength}; Actual: {length}.");
			return new SchemaValidationResults();
		}

		private static uint? _GetMaxLength(IJsonSchema schema)
		{
			return (schema as JsonSchema04)?.MaxLength ?? (schema as JsonSchema06)?.MaxLength;
		}
	}
}
