namespace Manatee.Json.Schema.Validators
{
	internal class IfThenElseSchema07PropertyValidator : IJsonSchemaPropertyValidator<JsonSchema07>
	{
		public bool Applies(JsonSchema07 schema, JsonValue json)
		{
			return schema.If != null || schema.Then != null || schema.Else != null;
		}
		public SchemaValidationResults Validate(JsonSchema07 schema, JsonValue json, JsonValue root)
		{
			if (schema.If == null) return new SchemaValidationResults();

			var ifResults = _ValidateSubSchema(schema.If, json, root);
			if (ifResults.Valid)
			{
				var thenResults = _ValidateSubSchema(schema.Then, json, root);
				if (thenResults.Valid) return new SchemaValidationResults();

				return new SchemaValidationResults("then", "Validation of `if` succeeded, but validation of `then` failed.");
			}

			var elseResults = _ValidateSubSchema(schema.Else, json, root);
			if (elseResults.Valid) return new SchemaValidationResults();

			return new SchemaValidationResults("else", "Validation of `if` failed, but validation of `else` also failed.");
		}

		private static SchemaValidationResults _ValidateSubSchema(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			return schema == null
				       ? new SchemaValidationResults()
				       : schema.Validate(json, root);
		}
	}
}