namespace Manatee.Json
{
	/// <summary>
	/// Specifies various types of values for use in a JSON key:value pair.
	/// </summary>
	public enum JsonValueType
	{
		/// <summary>
		/// Indicates that the Json key:value pair contains a numeric value (double).
		/// </summary>
		Number,
		/// <summary>
		/// Indicates that the Json key:value pair contains a string.
		/// </summary>
		String,
		/// <summary>
		/// Indicates that the Json key:value pair contains a boolean value.
		/// </summary>
		Boolean,
		/// <summary>
		/// Indicates that the Json key:value pair contains a nested Json object.
		/// </summary>
		Object,
		/// <summary>
		/// Indicates that the Json key:value pair contains a Json array.
		/// </summary>
		Array,
		/// <summary>
		/// Indicates that the Json key:value pair contains a null value.
		/// </summary>
		Null
	}
}