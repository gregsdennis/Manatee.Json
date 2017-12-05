using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class EnumSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract IEnumerable<EnumSchemaValue> GetEnum(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetEnum(schema) != null;
		}

		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var errors = GetEnum(schema).Select(d => d.Validate(json)).ToList();
			return errors.Any(r => r.Valid)
				? new SchemaValidationResults()
				: new SchemaValidationResults(errors);
		}
	}
	
	internal class Enumchema04PropertyValidator : EnumSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IEnumerable<EnumSchemaValue> GetEnum(JsonSchema04 schema)
		{
			return schema.Enum;
		}
	}
	
	internal class Enumchema06PropertyValidator : EnumSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IEnumerable<EnumSchemaValue> GetEnum(JsonSchema06 schema)
		{
			return schema.Enum;
		}
	}
	
	internal class Enumchema07PropertyValidator : EnumSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IEnumerable<EnumSchemaValue> GetEnum(JsonSchema07 schema)
		{
			return schema.Enum;
		}
	}
}
