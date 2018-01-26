namespace Manatee.Json
{
	/// <summary>
	/// Defines different kinds of array equality.
	/// </summary>
	public enum ArrayEquality
	{
		/// <summary>
		/// Defines that all elements in both arrays must match and be in the same sequence.
		/// </summary>
		SequenceEqual,
		/// <summary>
		/// Defines that all elements in both arrays much match, but may appear in any sequence.
		/// </summary>
		ContentsEqual
	}
}