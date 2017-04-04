namespace Manatee.Json.Serialization.Internal
{
	internal interface ISerializer
	{
		JsonValue Serialize<T>(T obj, JsonSerializer serializer);
		T Deserialize<T>(JsonValue json, JsonSerializer serializer);
		bool ShouldMaintainReferences { get; }
	}
}