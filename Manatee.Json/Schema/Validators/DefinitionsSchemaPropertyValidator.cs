using System.Collections.Generic;

namespace Manatee.Json.Schema.Validators
{
	internal class DefinitionsSchemaPropertyValidatorBase : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey("definitions");
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = new List<SchemaValidationError>();
			var definitions = json.Object["definitions"];
			if (definitions.Type != JsonValueType.Object)
				errors.Add(new SchemaValidationError("definitions", SchemaErrorMessages.Definitions));
			var metaSchema = JsonSchemaFactory.GetMetaSchema(schema.GetType());
			foreach (var value in definitions.Object.Values)
			{
				errors.AddRange(metaSchema.Validate(value).Errors);
			}
			return new SchemaValidationResults(errors);
		}
	}
}
