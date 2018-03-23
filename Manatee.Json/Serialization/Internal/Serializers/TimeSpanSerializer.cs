using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class TimeSpanSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(TimeSpan);
		}

		private static JsonValue _Encode(TimeSpan ts, JsonSerializer serializer)
		{
			return ts.ToString();
		}
		private static TimeSpan _Decode(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? TimeSpan.Parse(json.String) : default(TimeSpan);
		}
	}
}