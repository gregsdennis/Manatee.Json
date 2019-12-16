using System.Runtime.CompilerServices;

namespace Manatee.Json.Internal
{
	internal static class Log
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void General(string message)
		{
			if (JsonOptions.LogCategory.HasFlag(LogCategory.General))
				JsonOptions.Log?.Verbose(message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Schema(string message)
		{
			if (JsonOptions.LogCategory.HasFlag(LogCategory.Schema))
				JsonOptions.Log?.Verbose(message, LogCategory.Schema);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Serialization(string message)
		{
			if (JsonOptions.LogCategory.HasFlag(LogCategory.Serialization))
				JsonOptions.Log?.Verbose(message, LogCategory.Serialization);
		}
	}
}
