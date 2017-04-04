using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class EnumSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.Enum != null;
		}

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = schema.Enum.Select(d => d.Validate(json)).ToList();
			return errors.Any(r => r.Valid)
				? new SchemaValidationResults()
				: new SchemaValidationResults(errors);
		}
	}
}
