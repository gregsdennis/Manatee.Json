using System;

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

		public bool Handles(Type type, JsonSerializerOptions options)
		{
			return true;
		}
		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			if (Equals(obj, default(T)) && !serializer.Options.EncodeDefaultValues) return JsonValue.Null;
			return _innerSerializer.Serialize(obj, serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.Null
				       ? default(T)
				       : _innerSerializer.Deserialize<T>(json, serializer);
		}
	}
}