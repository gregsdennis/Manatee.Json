using System.Runtime.CompilerServices;

namespace Manatee.Json.Internal
{
	internal static class Log
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Verbose(string message, LogCategory category)
		{
			JsonOptions.Log?.Verbose(message, category);
		}
	}
}
