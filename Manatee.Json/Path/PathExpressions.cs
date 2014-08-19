namespace Manatee.Json.Path
{
	/// <summary>
	/// Defines extension methods which can be used within array and search JSON Path queries.
	/// </summary>
	public static class PathExpressions
	{
		/// <summary>
		/// Specifies the length of a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="json">The array.</param>
		/// <returns>The length of the array.</returns>
		public static int Length(this JsonArray json)
		{
			return json.Count;
		}
		/// <summary>
		/// Determines if an object contains a property or, if its value is a boolean, whether the value is true.
		/// </summary>
		/// <param name="json">The value.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>true if the value is an object and contains key <paramref name="name"/> or if its value is true; otherwise false.</returns>
		public static bool HasProperty(this JsonValue json, string name)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey(name);
		}
		/// <summary>
		/// Determines if an object contains a property containing a number and retrieves its value.
		/// </summary>
		/// <param name="json">The value.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value if the property exists and is a number; otherwise null.</returns>
		public static double? GetNumber(this JsonValue json, string name)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey(name) && json.Object[name].Type == JsonValueType.Number
					   ? json.Object[name].Number
					   : (double?)null;
		}
		/// <summary>
		/// Determines if an object contains a property containing a number and retrieves its value.
		/// </summary>
		/// <param name="json">The value.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value if the property exists and is a number; otherwise null.</returns>
		public static string GetString(this JsonValue json, string name)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey(name) && json.Object[name].Type == JsonValueType.String
					   ? json.Object[name].String
					   : null;
		}
	}
}