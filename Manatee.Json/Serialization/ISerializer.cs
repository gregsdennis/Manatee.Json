using System;
using Manatee.Json.Pointer;

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
		/// <param name="context"></param>
		/// <returns>true if the serializer is up to the task; false otherwise.</returns>
		bool Handles(SerializationContext context);
		/// <summary>
		/// Serializes a value.
		/// </summary>
		/// <typeparam name="T">The type of the value to serialize.</typeparam>
		/// <param name="context"></param>
		/// <returns>A <see cref="JsonValue"/> that represents the value.</returns>
		JsonValue Serialize<T>(SerializationContext<T> context);
		/// <summary>
		/// Deserializes a <see cref="JsonValue"/> into a value.
		/// </summary>
		/// <typeparam name="T">The type to be deserialized.</typeparam>
		/// <param name="context"></param>
		/// <param name="root">The root of the serialization.  Used for reference resolution.</param>
		/// <returns>The typed value represented by the JSON data.</returns>
		T Deserialize<T>(SerializationContext<JsonValue> context);
	}

	public class SerializationContext
	{
		public JsonSerializer RootSerializer { get; set; }
		public Type RequestedType { get; set; }
		public Type InferredType { get; set; }
		public JsonPointer CurrentLocation { get; set; }
	}

	public class SerializationContext<T> : SerializationContext
	{
		public T Source { get; set; }
	}
}