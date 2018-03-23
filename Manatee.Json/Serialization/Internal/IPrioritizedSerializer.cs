namespace Manatee.Json.Serialization.Internal
{
	internal interface IPrioritizedSerializer : ISerializer
	{
		int Priority { get; }
	}
}