using System;
using System.Reflection;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class JsonSerializableSerializer : IPrioritizedSerializer
	{
		public int Priority => -9;

		public bool ShouldMaintainReferences => true;

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return typeof(IJsonSerializable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			var serializable = (IJsonSerializable) obj;
			return serializable.ToJson(serializer);
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			var value = (IJsonSerializable)serializer.AbstractionMap.CreateInstance<T>(json, serializer.Options.Resolver);
			value.FromJson(json, serializer);
			return (T) value;
		}
	}
}