namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Enumeration of behavior options for the deserializer when a JSON structure is passed which
	/// contains invalid property keys.
	/// </summary>
	public enum InvalidPropertyKeyBehavior
	{
		/// <summary>
		/// Deserializer ignores the invalid property keys.
		/// </summary>
		DoNothing,
		/// <summary>
		/// Deserializer will throw an exception when an invalid property key is found.
		/// </summary>
		ThrowException
	}
}