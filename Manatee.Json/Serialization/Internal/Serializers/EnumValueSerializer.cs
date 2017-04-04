using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class EnumValueSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var value = Convert.ToInt32(obj);
			return value;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = (int) json.Number;
			return (T) Enum.ToObject(typeof (T), value);
		}
	}
}