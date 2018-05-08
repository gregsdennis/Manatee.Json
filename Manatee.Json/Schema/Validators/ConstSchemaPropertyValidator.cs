using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class ConstSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract JsonValue GetConst(T schema);

		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetConst(typed) != null;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var constant = GetConst((T)schema);
			if (constant != json)
			{
				var message = SchemaErrorMessages.ConstNotEqual.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = constant,
						["actual"] = json
					});
				return new SchemaValidationResults(string.Empty, message);
			}
			return new SchemaValidationResults();
		}
	}

	internal class ConstSchema06PropertyValidator : ConstSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override JsonValue GetConst(JsonSchema06 schema)
		{
			return schema.Const;
		}
	}

	internal class ConstSchema07PropertyValidator : ConstSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override JsonValue GetConst(JsonSchema07 schema)
		{
			return schema.Const;
		}
	}
}