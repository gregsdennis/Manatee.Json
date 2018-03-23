using System;

namespace Manatee.Json.Serialization
{
	public interface ISerializer
	{
		bool ShouldMaintainReferences { get; }

		bool Handles(Type type, JsonSerializerOptions options);
		JsonValue Serialize<T>(T obj, JsonSerializer serializer);
		T Deserialize<T>(JsonValue json, JsonSerializer serializer);
	}
}