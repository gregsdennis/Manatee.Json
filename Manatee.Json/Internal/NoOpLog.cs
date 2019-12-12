namespace Manatee.Json.Internal
{
	internal class NoOpLog : ILog
	{
		public static NoOpLog Instance { get; } = new NoOpLog();

		private NoOpLog() { }

		public void Verbose(string message) { }
	}

	internal static class Log
	{
		public static void Verbose(string message)
		{
			JsonOptions.Log.Verbose(message);
		}
	}
}