using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Optionally thrown when deserializing and the JSON structure contains property names
	/// which are not valid for the type requested.
	/// </summary>
	public class TypeDoesNotContainPropertyException : Exception
	{
		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type { get; }
		/// <summary>
		/// Gets the portion of the JSON structure which contain the invalid properties.
		/// </summary>
		public JsonValue Json { get; }
		/// <summary>
		/// Initializes a new instance of the <see cref="TypeDoesNotContainPropertyException"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="json">The invalid JSON structure.</param>
		internal TypeDoesNotContainPropertyException(Type type, JsonValue json)
			: base($"Type {type} does not contain any properties within {json}.")
		{
			Type = type;
			Json = json;
		}
	}
}