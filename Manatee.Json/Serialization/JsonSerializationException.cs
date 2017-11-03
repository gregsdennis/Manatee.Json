using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Thrown when an error occurs during serialization or deserialization.
	/// </summary>
	public class JsonSerializationException : Exception
	{
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSerializationException"/> class.
		/// </summary>
		public JsonSerializationException(string message, Exception innerException = null)
			: base(message, innerException) { }
	}
}
