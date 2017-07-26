using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class AllOfSchemaPropertyValidator : IJsonSchemaPropertyValidator<JsonSchema04>,
	                                              IJsonSchemaPropertyValidator<JsonSchema06>
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return schema.AllOf != null;
		}
		public bool Applies(JsonSchema06 schema, JsonValue json)
		{
			return schema.AllOf != null;
		}

		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationResults(schema.AllOf.Select(s => s.Validate(json, root)));
		}
		public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationResults(schema.AllOf.Select(s => s.Validate(json, root)));
		}
	}
}
