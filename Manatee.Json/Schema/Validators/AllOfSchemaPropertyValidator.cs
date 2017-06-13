using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class AllOfSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.AllOf != null;
		}

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationResults(schema.AllOf.Select(s => s.Validate(json, root)));
		}
	}
}
