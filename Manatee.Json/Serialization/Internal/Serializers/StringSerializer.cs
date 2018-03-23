using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class StringSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options)
		{
			return type == typeof(string);
		}
		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			return obj as string;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			return (T) (object) json.String;
		}
	}
}