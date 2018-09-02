using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal class DefinitionsSchemaPropertyValidatorBase : JsonSchemaPropertyValidator
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
			{
				var message = SchemaErrorMessages.Definitions.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = json
					});
				errors.Add(new SchemaValidationError("definitions", message));
			}

			var metaSchema = JsonSchemaFactory.GetMetaSchema(schema.GetType());
			foreach (var value in definitions.Object.Values)
			{
				errors.AddRange(metaSchema.Validate(value).Errors);
			}
			return new SchemaValidationResults(errors);
		}
	}
}
