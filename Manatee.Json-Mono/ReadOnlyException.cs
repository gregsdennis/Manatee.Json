using System;

namespace Manatee.Json
{
	public class ReadOnlyException : Exception
	{
		public ReadOnlyException(string message)
			: base(message) {}
	}
}
