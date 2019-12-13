using System;
using System.Threading.Tasks;

namespace Manatee.Json.Tests.Benchmark
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await PerformanceTests.Run();

			Console.ReadLine();
		}
	}
}
