using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class MaxLengthSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract uint? GetMaxLength(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetMaxLength(schema).HasValue && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var maxLength = GetMaxLength(schema);
			var length = new StringInfo(json.String).LengthInTextElements;
			if (maxLength.HasValue && length > maxLength)
				return new SchemaValidationResults(string.Empty, $"Expected: {maxLength}; Actual: {length}.");
			return new SchemaValidationResults();
		}
	}
	
	internal class MaxLengthSchema04PropertyValidator : MaxLengthSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMaxLength(JsonSchema04 schema)
		{
			return schema.MaxLength;
		}
	}
	
	internal class MaxLengthSchema06PropertyValidator : MaxLengthSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMaxLength(JsonSchema06 schema)
		{
			return schema.MaxLength;
		}
	}
}
