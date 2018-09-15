using System;
using Manatee.Json.Pointer;

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

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return true;
		}
		public JsonValue Serialize<T>(T obj, JsonPointer location, JsonSerializer serializer)
		{
			if (Equals(obj, default(T)) && !serializer.Options.EncodeDefaultValues) return JsonValue.Null;
			return _innerSerializer.Serialize(obj, location, serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonValue root, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.Null
				       ? default(T)
				       : _innerSerializer.Deserialize<T>(json, root, serializer);
		}
	}
}