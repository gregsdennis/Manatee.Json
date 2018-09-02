using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class ContainsSchemaPropertyValidatorBase<T> : JsonSchemaPropertyValidator
		where T : JsonSchema
	{
		protected abstract JsonSchema GetContains(T schema);

		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema is T typed && json.Type == JsonValueType.Array && GetContains(typed) != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var contains = GetContains((T)schema);
			if (json.Array.Count == 0)
			{
				var message = SchemaErrorMessages.Contains.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = contains,
						["value"] = json
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			var validations = json.Array.Select(jv => contains.Validate(jv, root)).ToList();
			if (!validations.Any(v => v.IsValid))
				return new SchemaValidationResults(validations);
			
			return new SchemaValidationResults();
		}
	}

	internal class ContainsSchema06PropertyValidator : ContainsSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override JsonSchema GetContains(JsonSchema06 schema)
		{
			return schema.Contains;
		}
	}

	internal class ContainsSchema07PropertyValidator : ContainsSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override JsonSchema GetContains(JsonSchema07 schema)
		{
			return schema.Contains;
		}
	}
}