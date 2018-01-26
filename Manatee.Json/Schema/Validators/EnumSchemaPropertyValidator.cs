using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class EnumSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract IEnumerable<EnumSchemaValue> GetEnum(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetEnum(typed) != null;
		}

		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = GetEnum((T)schema).Select(d => d.Validate(json)).ToList();
			return errors.Any(r => r.Valid)
				? new SchemaValidationResults()
				: new SchemaValidationResults(errors);
		}
	}
	
	internal class EnumSchema04PropertyValidator : EnumSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IEnumerable<EnumSchemaValue> GetEnum(JsonSchema04 schema)
		{
			return schema.Enum;
		}
	}
	
	internal class EnumSchema06PropertyValidator : EnumSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IEnumerable<EnumSchemaValue> GetEnum(JsonSchema06 schema)
		{
			return schema.Enum;
		}
	}
	
	internal class EnumSchema07PropertyValidator : EnumSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IEnumerable<EnumSchemaValue> GetEnum(JsonSchema07 schema)
		{
			return schema.Enum;
		}
	}
}
