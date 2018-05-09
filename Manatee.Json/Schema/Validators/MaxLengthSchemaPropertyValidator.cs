using System.Collections.Generic;
using System.Globalization;
using Manatee.Json.Internal;

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
			{
				var message = SchemaErrorMessages.MaxLength.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = maxLength,
						["actual"] = length,
						["value"] = json
				});
				return new SchemaValidationResults(string.Empty, message);
			}
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
