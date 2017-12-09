using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

[assembly:InternalsVisibleTo("Manatee.Json.DynamicTypes")]

namespace Manatee.Json.Tests.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			var stopwatch = new Stopwatch();

			var count = 1000;

			stopwatch.Start();

			for (int i = 0; i < count; i++)
			{
				_RunTest();
			}

			stopwatch.Stop();

			System.Console.WriteLine($"# of runs {count}.");
			System.Console.WriteLine($"Elapsed time: {stopwatch.Elapsed} ({stopwatch.ElapsedTicks}).");

			System.Console.ReadLine();
		}

		private static void _RunTest()
		{
			var text = File.ReadAllText("generated.json");
			var json = JsonValue.Parse(text);
		}
	}
}