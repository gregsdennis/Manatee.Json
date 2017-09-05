using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class AllOfSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract IEnumerable<IJsonSchema> GetAllOf(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetAllOf(schema) != null;
		}

		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationResults(GetAllOf(schema).Select(s => s.Validate(json, root)));
		}
	}

	internal class AllOfSchema04PropertyValidator : AllOfSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IEnumerable<IJsonSchema> GetAllOf(JsonSchema04 schema)
		{
			return schema.AllOf;
		}
	}
	
	internal class AllOfSchema06PropertyValidator : AllOfSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IEnumerable<IJsonSchema> GetAllOf(JsonSchema06 schema)
		{
			return schema.AllOf;
		}
	}
}
