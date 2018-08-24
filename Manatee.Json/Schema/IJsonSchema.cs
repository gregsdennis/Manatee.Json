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
		/// <summary>
		/// Used to specify which this schema defines.
		/// </summary>
		string Id { get; set; } // TODO: Make this read only
		/// <summary>
		/// Used to specify a schema which contains the definitions used by this schema.
		/// </summary>
		/// <remarks>
		/// if left null, the default of http://json-schema.org/draft-08/schema# is used.
		/// </remarks>
		string Schema { get; set; } // TODO: Make this read only
	}
}