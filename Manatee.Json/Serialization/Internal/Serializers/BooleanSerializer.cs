using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class BooleanSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options)
		{
			return type == typeof(bool);
		}
		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			return (bool) (object) obj;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			return (T) (object) json.Boolean;
		}
	}
}