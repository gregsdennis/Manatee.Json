using System;
using System.Diagnostics;

namespace Manatee.Json.Tests.Common
{
	public class ConsoleLog : ILog
	{
		private static readonly Stopwatch _stopwatch;

		static ConsoleLog()
		{
			_stopwatch = new Stopwatch();
			_stopwatch.Start();
		}

		public void Verbose(string message, LogCategory category)
		{
			if (JsonOptions.LogCategory.HasFlag(category))
				Console.WriteLine($@"[{_stopwatch.Elapsed:mm\:ss\.ffffff}]: {message}");
		}
	}
}
