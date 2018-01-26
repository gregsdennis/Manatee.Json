using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class MaxLengthSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract uint? GetMaxLength(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMaxLength(typed).HasValue && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var maxLength = GetMaxLength((T)schema);
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
	
	internal class MaxLengthSchema07PropertyValidator : MaxLengthSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override uint? GetMaxLength(JsonSchema07 schema)
		{
			return schema.MaxLength;
		}
	}
}
