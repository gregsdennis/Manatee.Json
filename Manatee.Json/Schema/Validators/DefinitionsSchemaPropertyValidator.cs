using System.Collections.Generic;

namespace Manatee.Json.Schema.Validators
{
	internal class DefinitionsSchemaPropertyValidator : IJsonSchemaPropertyValidator<JsonSchema04>,
	                                                    IJsonSchemaPropertyValidator<JsonSchema06>
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey("definitions");
		}
		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			var errors = new List<SchemaValidationError>();
			var definitions = json.Object["definitions"];
			if (definitions.Type != JsonValueType.Object)
				errors.Add(new SchemaValidationError("definitions", "Property 'definitions' must contain an object."));
			foreach (var value in definitions.Object.Values)
			{
				errors.AddRange(JsonSchema04.MetaSchema.Validate(value).Errors);
			}
			return new SchemaValidationResults(errors);
		}

		public bool Applies(JsonSchema06 schema, JsonValue json)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey("definitions");
		}
		public SchemaValidationResults Validate(JsonSchema06 schema, JsonValue json, JsonValue root)
		{
			var errors = new List<SchemaValidationError>();
			var definitions = json.Object["definitions"];
			if (definitions.Type != JsonValueType.Object)
				errors.Add(new SchemaValidationError("definitions", "Property 'definitions' must contain an object."));
			foreach (var value in definitions.Object.Values)
			{
				errors.AddRange(JsonSchema06.MetaSchema.Validate(value).Errors);
			}
			return new SchemaValidationResults(errors);
		}
	}
}
