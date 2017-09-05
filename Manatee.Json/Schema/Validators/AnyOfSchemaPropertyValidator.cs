using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class AnyOfSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract IEnumerable<IJsonSchema> GetAnyOf(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetAnyOf(schema) != null;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var errors = GetAnyOf(schema).Select(s => s.Validate(json, root)).ToList();
			return errors.Any(r => r.Valid)
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
}
