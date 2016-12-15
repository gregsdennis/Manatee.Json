using System;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when a read-only property is accessed.
	/// </summary>
	public class ReadOnlyException : Exception
	{
		/// <summary>
		/// Creates a new instance of the <see cref="ReadOnlyException"/> class.
		/// </summary>
		public ReadOnlyException(string message)
			: base(message) {}
	}
}
