using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class TimeSpanSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type == typeof(TimeSpan);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			var ts = (TimeSpan) (object) obj;

			return ts.ToString();
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			return json.Type == JsonValueType.String ? (T) (object) TimeSpan.Parse(json.String) : default(T);
		}
	}
}