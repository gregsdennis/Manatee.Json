using System.Threading.Tasks;

namespace Manatee.Json.Tests.Benchmark
{
	interface ICommandHandler
	{
		int Priority { get; }
		string Command { get; }

		Task Run(string value);
	}
}