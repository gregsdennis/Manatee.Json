using System.Threading.Tasks;

namespace Manatee.Json.Tests.Benchmark
{
	class SerializationCommandHandler : ICommandHandler
	{
		public int Priority => 1;
		public string Command => "--serializer";

		public Task Run(string value)
		{
			return PerformanceTests.Run();
		}
	}
}