using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class MinLengthSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract uint? GetMinLength(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMinLength(typed).HasValue && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var minLength = GetMinLength((T)schema);
			var length = new StringInfo(json.String).LengthInTextElements;
			if (minLength.HasValue && length < minLength)
				return new SchemaValidationResults(string.Empty, $"Expected: length >= {minLength}; Actual: {length}.");
			return new SchemaValidationResults();
		}
	}
	
	internal class MinLengthSchema04PropertyValidator : MinLengthSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMinLength(JsonSchema04 schema)
		{
			return schema.MinLength;
		}
	}
	
	internal class MinLengthSchema06PropertyValidator : MinLengthSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMinLength(JsonSchema06 schema)
		{
			return schema.MinLength;
		}
	}
	
	internal class MinLengthSchema07PropertyValidator : MinLengthSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override uint? GetMinLength(JsonSchema07 schema)
		{
			return schema.MinLength;
		}
	}
}
