namespace Manatee.Json.Serialization.Internal
{
	internal interface ISerializer
	{
		bool ShouldMaintainReferences { get; }

		JsonValue Serialize<T>(T obj, JsonSerializer serializer);
		T Deserialize<T>(JsonValue json, JsonSerializer serializer);
	}
}