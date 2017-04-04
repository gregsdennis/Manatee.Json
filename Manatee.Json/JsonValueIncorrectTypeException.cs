using System;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when a value is accessed via the incorrect type accessor.
	/// </summary>
#if !IOS && !CORE
	[Serializable]
#endif
	public class JsonValueIncorrectTypeException : InvalidOperationException
	{
		/// <summary>
		/// The correct type for the <see cref="JsonValue"/> that threw the exception.
		/// </summary>
		public JsonValueType ValidType { get; }
		/// <summary>
		/// The type requested.
		/// </summary>
		public JsonValueType RequestedType { get; }
		/// <summary>
		/// Creates a new instance of this exception.
		/// </summary>
		internal JsonValueIncorrectTypeException(JsonValueType valid, JsonValueType requested)
			: base($"Cannot access value of type {valid} as type {requested}.")
		{
			ValidType = valid;
			RequestedType = requested;
		}
	}
}