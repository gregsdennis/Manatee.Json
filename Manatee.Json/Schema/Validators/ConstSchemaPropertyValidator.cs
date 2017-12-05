namespace Manatee.Json.Schema.Validators
{
	internal abstract class ConstSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract JsonValue GetConst(T schema);

		public bool Applies(T schema, JsonValue json)
		{
			return GetConst(schema) != null;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var constant = GetConst(schema);
			if (constant != json)
				return new SchemaValidationResults(string.Empty, $"Expected: {constant}; Actual: {json}.");
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