using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class OneOfSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract IEnumerable<IJsonSchema> GetOneOf(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetOneOf(schema) != null;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var errors = GetOneOf(schema).Select(s => s.Validate(json, root)).ToList();
			var validCount = errors.Count(r => r.Valid);
			switch (validCount)
			{
				case 0:
					return new SchemaValidationResults(errors);
				case 1:
					return new SchemaValidationResults();
				default:
					return new SchemaValidationResults(string.Empty, "More than one option was valid.");
			}
		}
	}
	
	internal class OneOfSchema04PropertyValidator : OneOfSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IEnumerable<IJsonSchema> GetOneOf(JsonSchema04 schema)
		{
			return schema.OneOf;
		}
	}
	
	internal class OneOfSchema06PropertyValidator : OneOfSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IEnumerable<IJsonSchema> GetOneOf(JsonSchema06 schema)
		{
			return schema.OneOf;
		}
	}
	
	internal class OneOfSchema07PropertyValidator : OneOfSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IEnumerable<IJsonSchema> GetOneOf(JsonSchema07 schema)
		{
			return schema.OneOf;
		}
	}
}
