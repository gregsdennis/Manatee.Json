using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class DependenciesSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.Dependencies != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationResults(schema.Dependencies.SelectMany(d => d.Validate(json, root).Errors));
		}
	}
}
