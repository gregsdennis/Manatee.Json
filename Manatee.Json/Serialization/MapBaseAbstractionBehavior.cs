namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Describes mapping behaviors for mapping abstraction types in the serializer.
	/// </summary>
	public enum MapBaseAbstractionBehavior
	{
		/// <summary>
		/// Specifies that no additional mappings will be made.
		/// </summary>
		None,
		/// <summary>
		/// Specifies that any unmapped base classes and interfaces will be mapped.
		/// </summary>
		Unmapped,
		/// <summary>
		/// Specifies that all base classes and interfaces will be mapped, overriding any existing mappings.
		/// </summary>
		Override
	}
}