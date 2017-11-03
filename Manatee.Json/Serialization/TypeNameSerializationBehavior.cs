namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Enumerates serialization behaviors for saving type names.
	/// </summary>
	public enum TypeNameSerializationBehavior
	{
		/// <summary>
		/// Serializes the type name as necessary.
		/// </summary>
		Auto,
		/// <summary>
		/// Serializes the type name only for abstract and interface types.
		/// </summary>
		OnlyForAbstractions,
		/// <summary>
		/// Always serializes the type name.
		/// </summary>
		Always,
		/// <summary>
		/// Never serializes the type name.
		/// </summary>
		Never
	}
}