using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class MinLengthSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract uint? GetMinLength(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetMinLength(schema).HasValue && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var length = new StringInfo(json.String).LengthInTextElements;
			if (GetMinLength(schema).HasValue && length < GetMinLength(schema))
				return new SchemaValidationResults(string.Empty, $"Expected: length >= {GetMinLength(schema)}; Actual: {length}.");
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
