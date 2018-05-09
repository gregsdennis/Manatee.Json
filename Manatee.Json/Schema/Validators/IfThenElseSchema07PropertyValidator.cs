namespace Manatee.Json.Schema.Validators
{
	internal class IfThenElseSchema07PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema07 typed && (typed.If != null || typed.Then != null || typed.Else != null);
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema07) schema;
			if (typed.If == null) return new SchemaValidationResults();

			var ifResults = _ValidateSubSchema(typed.If, json, root);
			if (ifResults.Valid)
			{
				var thenResults = _ValidateSubSchema(typed.Then, json, root);
				if (thenResults.Valid) return new SchemaValidationResults();

				return new SchemaValidationResults("then", SchemaErrorMessages.Then);
			}

			var elseResults = _ValidateSubSchema(typed.Else, json, root);
			if (elseResults.Valid) return new SchemaValidationResults();

			return new SchemaValidationResults("else", SchemaErrorMessages.Else);
		}

		private static SchemaValidationResults _ValidateSubSchema(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			return schema == null
				       ? new SchemaValidationResults()
				       : schema.Validate(json, root);
		}
	}
}