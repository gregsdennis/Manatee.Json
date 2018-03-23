using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class StringSerializer : IPrioritizedSerializer
	{
		public int Priority => int.MinValue;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
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