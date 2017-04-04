using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a type for all schema to implement.
	/// </summary>
	public interface IJsonSchema : IJsonSerializable, IEquatable<IJsonSchema>
	{
		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		SchemaValidationResults Validate(JsonValue json, JsonValue root = null);
		/// <summary>
		/// Identifies the physical path for the schema document (may be different than the ID).
		/// </summary>
		Uri DocumentPath { get; set; }
	}
}