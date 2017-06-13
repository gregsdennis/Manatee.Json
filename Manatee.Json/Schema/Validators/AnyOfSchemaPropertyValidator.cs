using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class AnyOfSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.AnyOf != null;
		}

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = schema.AnyOf.Select(s => s.Validate(json, root)).ToList();
			return errors.Any(r => r.Valid)
					   ? new SchemaValidationResults()
					   : new SchemaValidationResults(errors);
		}
	}
}
