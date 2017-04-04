using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class OneOfSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.OneOf != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = schema.OneOf.Select(s => s.Validate(json, root)).ToList();
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
}
