using System;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Enumerates the official schema versions (drafts).
	/// </summary>
	[Flags]
	public enum JsonSchemaVersion
	{
		/// <summary>
		/// No adherence to any known schema draft.
		/// </summary>
		None = 0,
		/// <summary>
		/// Adheres to draft-04.
		/// </summary>
		Draft04 = 1 << 0,
		/// <summary>
		/// Adheres to draft-06.
		/// </summary>
		Draft06 = 1 << 1,
		/// <summary>
		/// Adheres to draft-07.
		/// </summary>
		Draft07 = 1 << 2,
		/// <summary>
		/// Adheres to 2019-04.
		/// </summary>
		Draft2019_04 = 1 << 3,
		/// <summary>
		/// Adheres to all known schema drafts.
		/// </summary>
		All = Draft04 | Draft06 | Draft07 | Draft2019_04
	}
}