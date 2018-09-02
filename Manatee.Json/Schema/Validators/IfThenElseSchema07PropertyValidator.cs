using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal class IfThenElseSchema07PropertyValidator : JsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema07 typed && (typed.If != null || typed.Then != null || typed.Else != null);
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema07) schema;
			if (typed.If == null) return new SchemaValidationResults();

			var ifResults = _ValidateSubSchema(typed.If, json, root);
			string message;
			if (ifResults.IsValid)
			{
				var thenResults = _ValidateSubSchema(typed.Then, json, root);
				if (thenResults.IsValid) return new SchemaValidationResults();

				message = SchemaErrorMessages.Then.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = json
					});
				return new SchemaValidationResults("then", message);
			}

			var elseResults = _ValidateSubSchema(typed.Else, json, root);
			if (elseResults.IsValid) return new SchemaValidationResults();

			message = SchemaErrorMessages.Else.ResolveTokens(new Dictionary<string, object>
				{
					["value"] = json
				});
			return new SchemaValidationResults("else", message);
		}

		private static SchemaValidationResults _ValidateSubSchema(JsonSchema schema, JsonValue json, JsonValue root)
		{
			return schema == null
				       ? new SchemaValidationResults()
				       : schema.Validate(json, root);
		}
	}
}