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

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return true;
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			if (Equals(obj, default(T)) && !serializer.Options.EncodeDefaultValues) return JsonValue.Null;
			return _innerSerializer.Serialize(context, location);
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			return json.Type == JsonValueType.Null
				       ? default(T)
				       : _innerSerializer.Deserialize<T>(context, root);
		}
	}
}