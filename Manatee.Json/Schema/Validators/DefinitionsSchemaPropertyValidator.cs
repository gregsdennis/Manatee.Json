using System.Collections.Generic;

namespace Manatee.Json.Schema.Validators
{
	internal class DefinitionsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey("definitions");
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = new List<SchemaValidationError>();
			var definitions = json.Object["definitions"];
			if (definitions.Type != JsonValueType.Object)
				errors.Add(new SchemaValidationError("definitions", "Property 'definitions' must contain an object."));
			foreach (var value in definitions.Object.Values)
			{
				errors.AddRange(JsonSchema.Draft04.Validate(value).Errors);
			}
			return new SchemaValidationResults(errors);
		}
	}
}
