using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class ContainsSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract IJsonSchema GetContains(T schema);

		public bool Applies(T schema, JsonValue json)
		{
			return json.Type == JsonValueType.Array && GetContains(schema) != null;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var contains = GetContains(schema);
			if (json.Array.Count == 0)
				return new SchemaValidationResults(string.Empty, $"Expected an item that matched '{contains}' but no items were found.");

			var validations = json.Array.Select(jv => contains.Validate(jv, root)).ToList();
			if (!validations.Any(v => v.Valid))
				return new SchemaValidationResults(validations);
			
			return new SchemaValidationResults();
		}
	}

	internal class ContainsSchema06PropertyValidator : ContainsSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IJsonSchema GetContains(JsonSchema06 schema)
		{
			return schema.Contains;
		}
	}

	internal class ContainsSchema07PropertyValidator : ContainsSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IJsonSchema GetContains(JsonSchema07 schema)
		{
			return schema.Contains;
		}
	}
}