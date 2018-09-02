using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class AllOfSchemaPropertyValidatorBase<T> : JsonSchemaPropertyValidator
		where T : JsonSchema
	{
		protected abstract IEnumerable<JsonSchema> GetAllOf(T schema);
		
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetAllOf(typed) != null;
		}

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationResults(GetAllOf((T) schema).Select(s => s.Validate(json, root)));
		}
	}

	internal class AllOfSchema04PropertyValidator : AllOfSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IEnumerable<JsonSchema> GetAllOf(JsonSchema04 schema)
		{
			return schema.AllOf;
		}
	}
	
	internal class AllOfSchema06PropertyValidator : AllOfSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IEnumerable<JsonSchema> GetAllOf(JsonSchema06 schema)
		{
			return schema.AllOf;
		}
	}
	
	internal class AllOfSchema07PropertyValidator : AllOfSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IEnumerable<JsonSchema> GetAllOf(JsonSchema07 schema)
		{
			return schema.AllOf;
		}
	}
}
