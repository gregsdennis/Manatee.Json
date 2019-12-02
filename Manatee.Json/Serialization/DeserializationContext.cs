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
		public Dictionary<SerializationInfo, object> ValueMap { get; set; }

		internal DeserializationContext(JsonSerializer rootSerializer, JsonValue jsonRoot)
			: base(rootSerializer)
		{
			JsonRoot = jsonRoot;
		}

		public void Push(Type type, string propertyName, JsonValue localValue)
		{
			base.Push(type, type, propertyName);

			_localValues.Push(localValue);
			LocalValue = localValue;
		}
		public void Pop()
		{
			base.Pop();

			LocalValue = _localValues.Pop();
		}
	}
}