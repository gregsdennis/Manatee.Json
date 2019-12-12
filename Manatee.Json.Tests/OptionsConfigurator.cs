using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[SetUpFixture]
	public class OptionsConfigurator
	{
		class ConsoleLog : ILog
		{
			private static readonly Stopwatch _stopwatch;

			static ConsoleLog()
			{
				_stopwatch = new Stopwatch();
				_stopwatch.Start();
			}

			public void Verbose(string message)
			{
				Console.WriteLine($@"[{_stopwatch.Elapsed:mm\:ss\.ffffff}]: {message}");
			}
		}

		[OneTimeSetUp]
		public void Setup()
		{
			JsonOptions.Log = new ConsoleLog();
		}
	}
}	