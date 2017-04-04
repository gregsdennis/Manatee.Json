using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Enumerates the types of properties which are automatically serialized.
	/// </summary>
	[Flags]
	public enum PropertySelectionStrategy
	{
		/// <summary>
		/// Indicates that read/write properties will be serialized.
		/// </summary>
		ReadWriteOnly = 0x01,
		/// <summary>
		/// Indicates that read-only properties will be serialized.
		/// </summary>
		ReadOnly = 0x02,
	}
}