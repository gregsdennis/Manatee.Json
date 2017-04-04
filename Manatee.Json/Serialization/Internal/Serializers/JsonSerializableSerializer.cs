namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class JsonSerializableSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => true;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var serializable = (IJsonSerializable) obj;
			return serializable.ToJson(serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = (IJsonSerializable) JsonSerializationAbstractionMap.CreateInstance<T>(json, serializer.Options.Resolver);
			value.FromJson(json, serializer);
			return (T) value;
		}
	}
}