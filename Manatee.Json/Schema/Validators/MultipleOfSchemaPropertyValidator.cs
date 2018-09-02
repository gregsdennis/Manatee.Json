using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class MultipleOfSchemaPropertyValidatorBase<T> : JsonSchemaPropertyValidator
		where T : JsonSchema
	{
		protected abstract double? GetMultipleOf(T schema);
		
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMultipleOf(typed).HasValue && json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var multipleOf = GetMultipleOf((T) schema);
			if ((decimal) json.Number % (decimal?) multipleOf != 0)
			{
				var message = SchemaErrorMessages.MultipleOf.ResolveTokens(new Dictionary<string, object>
					{
						["multipleOf"] = multipleOf,
						["actual"] = json.Number % multipleOf,
						["value"] = json
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return new SchemaValidationResults();
		}
	}
	
	internal class MultipleOfSchema04PropertyValidator : MultipleOfSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override double? GetMultipleOf(JsonSchema04 schema)
		{
			return schema.MultipleOf;
		}
	}
	
	internal class MultipleOfSchema06PropertyValidator : MultipleOfSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override double? GetMultipleOf(JsonSchema06 schema)
		{
			return schema.MultipleOf;
		}
	}
	
	internal class MultipleOfSchema07PropertyValidator : MultipleOfSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override double? GetMultipleOf(JsonSchema07 schema)
		{
			return schema.MultipleOf;
		}
	}
}
