using System;
using System.Reflection;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class JsonSerializableSerializer : IPrioritizedSerializer
	{
		public int Priority => -9;

		public bool ShouldMaintainReferences => true;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return typeof(IJsonSerializable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
		}
		public JsonValue Serialize<T>(T obj, JsonPointer location, JsonSerializer serializer)
		{
			var serializable = (IJsonSerializable) obj;
			return serializable.ToJson(serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonValue root, JsonSerializer serializer)
		{
			var value = (IJsonSerializable)serializer.AbstractionMap.CreateInstance<T>(json, serializer.Options.Resolver);
			value.FromJson(json, serializer);
			return (T) value;
		}
	}
}