namespace Manatee.Json.Schema
{
	/// <summary>
	/// Performs validations for a single schema property.
	/// </summary>
	public interface JsonSchemaPropertyValidator
	{
		/// <summary>
		/// Determines whether the validator should execute for a particular schema and JSON instance.
		/// </summary>
		/// <param name="schema">The schema.</param>
		/// <param name="json">The JSON instance.</param>
		/// <returns>true if the validatory should execute; otherwise false.</returns>
		bool Applies(JsonSchema schema, JsonValue json);
		/// <summary>
		/// Performs validations of a schema property on a JSON instance.
		/// </summary>
		/// <param name="schema">The schema.</param>
		/// <param name="json">The JSON instance.</param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used resolving references. Pass this to any subschema validations that need to be performed.</param>
		/// <returns>A <see cref="SchemaValidationResults"/> instance.</returns>
		SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root);
	}
}
