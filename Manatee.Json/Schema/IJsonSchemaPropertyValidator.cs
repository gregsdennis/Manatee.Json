namespace Manatee.Json.Schema
{
	public interface IJsonSchemaPropertyValidator { }

	public interface IJsonSchemaPropertyValidator<in T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		bool Applies(T schema, JsonValue json);
		SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root);
	}
}
