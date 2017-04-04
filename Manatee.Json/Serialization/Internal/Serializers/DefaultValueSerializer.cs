namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DefaultValueSerializer : ISerializer
	{
		private readonly ISerializer _innerSerializer;

		public bool ShouldMaintainReferences => _innerSerializer.ShouldMaintainReferences;

		public DefaultValueSerializer(ISerializer innerSerializer)
		{
			_innerSerializer = innerSerializer;
		}

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			if (Equals(obj, default(T)) && !serializer.Options.EncodeDefaultValues) return JsonValue.Null;
			return _innerSerializer.Serialize(obj, serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Null) return default(T);
			return _innerSerializer.Deserialize<T>(json, serializer);
		}
	}
}