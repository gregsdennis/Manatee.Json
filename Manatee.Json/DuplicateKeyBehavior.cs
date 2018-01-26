namespace Manatee.Json
{
	/// <summary>
	/// Defines behavior of <see cref="JsonObject"/> when adding items at already exist.
	/// </summary>
	public enum DuplicateKeyBehavior
	{
		/// <summary>
		/// Throw an exception.
		/// </summary>
		Throw,
		/// <summary>
		/// Overwrite the existing item.
		/// </summary>
		Overwrite
	}
}