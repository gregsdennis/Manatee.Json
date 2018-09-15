using System;
using System.Reflection;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class EnumValueSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type.GetTypeInfo().IsEnum &&
			       (options.EnumSerializationFormat == EnumSerializationFormat.AsInteger ||		// used during serialization
			        json?.Type == JsonValueType.Number);										// used during deserialization
		}
		public JsonValue Serialize<T>(T obj, JsonPointer location, JsonSerializer serializer)
		{
			var value = Convert.ToInt32(obj);
			return value;
		}
		public T Deserialize<T>(JsonValue json, JsonValue root, JsonSerializer serializer)
		{
			var value = (int) json.Number;
			return (T) Enum.ToObject(typeof (T), value);
		}
	}
}