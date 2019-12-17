using System;
using System.Collections.Generic;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Encapsulates all data required to deserialize an object from JSON.
	/// </summary>
	public class DeserializationContext : SerializationContextBase
	{
		private readonly Stack<JsonValue> _localValues = new Stack<JsonValue>();

		/// <summary>
		/// The root of the JSON instance.
		/// </summary>
		public JsonValue JsonRoot { get; }
		/// <summary>
		/// The current value in the JSON instance for deserialization.
		/// </summary>
		public JsonValue LocalValue { get; private set; }
		/// <summary>
		/// A mapping of the deserialized values to the type's property information.
		/// </summary>
		public Dictionary<SerializationInfo, object?> ValueMap { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		internal DeserializationContext(JsonSerializer rootSerializer, JsonValue jsonRoot)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
			: base(rootSerializer)
		{
			JsonRoot = jsonRoot;
		}

		/// <summary>
		/// Pushes new details onto the context to allow for recursive serialization.
		/// </summary>
		/// <param name="type">The type to be deserialized.</param>
		/// <param name="propertyName">The property name or index.  Will be appended to the location as a JSON Path segment.</param>
		/// <param name="localValue">The local JSON value being deserialized.</param>
		public void Push(Type type, string propertyName, JsonValue localValue)
		{
			PushDetails(type, type, propertyName);

			_localValues.Push(LocalValue);
			LocalValue = localValue;
		}
		/// <summary>
		/// Pops details from the context to keep the context in sync with the deserialization process.
		/// </summary>
		public void Pop()
		{
			PopDetails();

			LocalValue = _localValues.Pop();
		}
	}
}