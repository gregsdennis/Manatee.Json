using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class NotSchemaPropertyValidatorBase<T> : JsonSchemaPropertyValidator
		where T : JsonSchema
	{
		protected abstract JsonSchema GetNot(T schema);
		
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetNot(typed) != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var results = GetNot((T)schema).Validate(json, root);
			if (results.IsValid)
			{
				var message = SchemaErrorMessages.Not.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = json
					});
				return new SchemaValidationResults(string.Empty, message);
			}

			return new SchemaValidationResults();
		}
	}
	
	internal class NotSchema04PropertyValidator : NotSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override JsonSchema GetNot(JsonSchema04 schema)
		{
			return schema.Not;
		}
	}
	
	internal class NotSchema06PropertyValidator : NotSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override JsonSchema GetNot(JsonSchema06 schema)
		{
			return schema.Not;
		}
	}
	
	internal class NotSchema07PropertyValidator : NotSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override JsonSchema GetNot(JsonSchema07 schema)
		{
			return schema.Not;
		}
	}
}
