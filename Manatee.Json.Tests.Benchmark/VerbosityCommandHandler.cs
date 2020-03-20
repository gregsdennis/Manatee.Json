using System.Threading.Tasks;
using Manatee.Json.Tests.Common;

namespace Manatee.Json.Tests.Benchmark
{
	class VerbosityCommandHandler : ICommandHandler
	{
		public int Priority => 0;
		public string Command => "--verbose";

		public Task Run(string value)
		{
			JsonOptions.Log = new ConsoleLog();
			return Task.CompletedTask;
		}
	}
}