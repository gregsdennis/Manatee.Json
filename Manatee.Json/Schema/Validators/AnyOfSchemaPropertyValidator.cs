using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class AnyOfSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract IEnumerable<IJsonSchema> GetAnyOf(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetAnyOf(typed) != null;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = GetAnyOf((T)schema).Select(s => s.Validate(json, root)).ToList();
			return errors.Any(r => r.IsValid)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(errors);
		}
	}
	
	internal class AnyOfSchema04PropertyValidator : AnyOfSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IEnumerable<IJsonSchema> GetAnyOf(JsonSchema04 schema)
		{
			return schema.AnyOf;
		}
	}
	
	internal class AnyOfSchema06PropertyValidator : AnyOfSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IEnumerable<IJsonSchema> GetAnyOf(JsonSchema06 schema)
		{
			return schema.AnyOf;
		}
	}
	
	internal class AnyOfSchema07PropertyValidator : AnyOfSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IEnumerable<IJsonSchema> GetAnyOf(JsonSchema07 schema)
		{
			return schema.AnyOf;
		}
	}
}
