using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class TimeSpanSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(TimeSpan);
		}
		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var ts = (TimeSpan) (object) obj;

			return ts.ToString();
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? (T) (object) TimeSpan.Parse(json.String) : default(T);
		}
	}
}