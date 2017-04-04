namespace Manatee.Json.Serialization.Internal
{
	internal interface ITypeSerializer
	{
		JsonValue SerializeType<T>(JsonSerializer serializer);
		void DeserializeType<T>(JsonValue json, JsonSerializer serializer);
	}
}