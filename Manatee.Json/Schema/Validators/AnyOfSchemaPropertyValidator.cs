using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class AnyOfSchemaPropertyValidator : IJsonSchemaPropertyValidator<JsonSchema04>,
	                                              IJsonSchemaPropertyValidator<JsonSchema06>
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return schema.AnyOf != null;
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			var errors = schema.AnyOf.Select(s => s.Validate(json, root)).ToList();
			return errors.Any(r => r.Valid)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(errors);
		}

		public bool Applies(JsonSchema06 schema, JsonValue json)
		{
			return schema.AnyOf != null;
		}
		public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
		{
			var errors = schema.AnyOf.Select(s => s.Validate(json, root)).ToList();
			return errors.Any(r => r.Valid)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(errors);
		}
	}
}
