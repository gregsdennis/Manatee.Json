using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class GuidSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(Guid);
		}

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var guid = (Guid) (object) obj;
			return guid.ToString();
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? (T) (object) new Guid(json.String) : default(T);
		}
	}
}