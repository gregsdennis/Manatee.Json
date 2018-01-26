using System.Collections.Generic;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class ExamplesSchemaPropertyValidator<T> : IJsonSchemaPropertyValidator
	{
		public abstract JsonArray GetExamples(T schema);

		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetExamples(typed) != null;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Type != JsonValueType.Array)
				return new SchemaValidationResults(string.Empty, $"Expected: Array; Actual: {json.Type}");
			return new SchemaValidationResults();
		}
	}

	internal class ExamplesSchema06PropertyValidator : ExamplesSchemaPropertyValidator<JsonSchema06>
	{
		public override JsonArray GetExamples(JsonSchema06 schema)
		{
			return schema.Examples;
		}
	}

	internal class ExamplesSchema07PropertyValidator : ExamplesSchemaPropertyValidator<JsonSchema07>
	{
		public override JsonArray GetExamples(JsonSchema07 schema)
		{
			return schema.Examples;
		}
	}
}