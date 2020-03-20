using System.Threading.Tasks;

namespace Manatee.Json.Tests.Benchmark
{
	class SchemaCommandHandler : ICommandHandler
	{
		public int Priority => 1;
		public string Command => "--schema";

		public Task Run(string value)
		{
			return SchemaPerformance.Run();
		}
	}
}