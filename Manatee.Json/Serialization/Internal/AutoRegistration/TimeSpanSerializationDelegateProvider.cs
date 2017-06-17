using System;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class TimeSpanSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
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