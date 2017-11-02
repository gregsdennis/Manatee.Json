using System;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Provides extension methods which can be used within array and search JSON Path queries.
	/// </summary>
	public static class JsonPathRoot
	{
		/// <summary>
		/// Specifies the length of a <see cref="JsonArray"/>.
		/// </summary>
		/// <returns>The length of the array.</returns>
		public static int Length()
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property or, if its value is a boolean, whether the value is true.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>true if the value is an object and contains key <paramref name="name"/> or if its value is true; otherwise false.</returns>
		public static bool HasProperty(string name)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property containing a number and retrieves its value.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value if the property exists and is a number; otherwise null.</returns>
		public static JsonPathValue Name(string name)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property containing a number and retrieves its value.
		/// </summary>
		/// <param name="index">The index to retreive.</param>
		/// <returns>The value if the property exists and is a number; otherwise null.</returns>
		public static JsonPathValue ArrayIndex(int index)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
	}
}