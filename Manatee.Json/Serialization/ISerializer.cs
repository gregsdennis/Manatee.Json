using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Defines a custom serializer.
	/// </summary>
	public interface ISerializer
	{
		/// <summary>
		/// Determines if this serializer should maintain referential integrity.
		/// </summary>
		bool ShouldMaintainReferences { get; }

		/// <summary>
		/// Determines whether the serializer handles a specific type or JSON value given the current options.
		/// </summary>
		/// <param name="type">The requested object type.</param>
		/// <param name="options">The serializer options.</param>
		/// <param name="json">The JSON instance being deserialized.</param>
		/// <returns>true if the serializer is up to the task; false otherwise.</returns>
		bool Handles(Type type, JsonSerializerOptions options, JsonValue json);
		/// <summary>
		/// Serializes a value.
		/// </summary>
		/// <typeparam name="T">The type of the value to serialize.</typeparam>
		/// <param name="obj">The value to serialize.</param>
		/// <param name="serializer">The primary serializer instance.  Provided for nested object serialization.</param>
		/// <returns>A <see cref="JsonValue"/> that represents the value.</returns>
		JsonValue Serialize<T>(T obj, JsonSerializer serializer);
		/// <summary>
		/// Deserializes a <see cref="JsonValue"/> into a value.
		/// </summary>
		/// <typeparam name="T">The type to be deserialized.</typeparam>
		/// <param name="json">The JSON data to deserialize.</param>
		/// <param name="serializer">The primary serializer instance.  Provided for nested object serialization.</param>
		/// <returns>The typed value represented by the JSON data.</returns>
		T Deserialize<T>(JsonValue json, JsonSerializer serializer);
	}
}