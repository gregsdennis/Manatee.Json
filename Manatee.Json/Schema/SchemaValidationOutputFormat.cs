namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the different output verbosities supported.
	/// </summary>
	public enum SchemaValidationOutputFormat
	{
		/// <summary>
		/// Denotes that only a true/false value will be returned.
		/// </summary>
		Flag,
		/// <summary>
		/// Denotes that the errors will be returned as a flat list.
		/// </summary>
		Basic,
		/// <summary>
		/// Denotes that the errors will appear in a collapsed hierarchy.
		/// </summary>
		Detailed,
		/// <summary>
		/// Denotes that the errors will appear in a raw, uncollapsed hierarchy.
		/// </summary>
		Verbose
	}
}