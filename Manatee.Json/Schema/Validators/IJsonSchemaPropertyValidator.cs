namespace Manatee.Json.Schema.Validators
{
	internal interface IJsonSchemaPropertyValidator { }

	internal interface IJsonSchemaPropertyValidator<in T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		bool Applies(T schema, JsonValue json);
		SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root);
	}
}
