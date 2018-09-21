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

		public bool Handles(SerializationContext context)
		{
			return true;
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			if (Equals(context.Source, default(T)) && !context.RootSerializer.Options.EncodeDefaultValues) return JsonValue.Null;
			return _innerSerializer.Serialize(context);
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			return context.Source.Type == JsonValueType.Null
				       ? default(T)
				       : _innerSerializer.Deserialize<T>(context);
		}
	}
}