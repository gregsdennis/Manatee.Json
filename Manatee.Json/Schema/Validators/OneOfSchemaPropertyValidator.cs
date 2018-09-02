using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class OneOfSchemaPropertyValidatorBase<T> : JsonSchemaPropertyValidator
		where T : JsonSchema
	{
		protected abstract IEnumerable<JsonSchema> GetOneOf(T schema);
		
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetOneOf(typed) != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = GetOneOf((T)schema).Select(s => s.Validate(json, root)).ToList();
			var validCount = errors.Count(r => r.IsValid);
			switch (validCount)
			{
				case 0:
					return new SchemaValidationResults(errors);
				case 1:
					return new SchemaValidationResults();
				default:
					var message = SchemaErrorMessages.OneOf.ResolveTokens(new Dictionary<string, object>
						{
							["value"] = json
						});
					return new SchemaValidationResults(string.Empty, message);
			}
		}
	}
	
	internal class OneOfSchema04PropertyValidator : OneOfSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IEnumerable<JsonSchema> GetOneOf(JsonSchema04 schema)
		{
			return schema.OneOf;
		}
	}
	
	internal class OneOfSchema06PropertyValidator : OneOfSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IEnumerable<JsonSchema> GetOneOf(JsonSchema06 schema)
		{
			return schema.OneOf;
		}
	}
	
	internal class OneOfSchema07PropertyValidator : OneOfSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IEnumerable<JsonSchema> GetOneOf(JsonSchema07 schema)
		{
			return schema.OneOf;
		}
	}
}
