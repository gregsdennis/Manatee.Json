using System;
using System.Globalization;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DateTimeSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(DateTime);
		}
		public JsonValue Serialize<T>(T obj, JsonPointer location, JsonSerializer serializer)
		{
			var dt = (DateTime) (object) obj;
			if (serializer.Options == null)
				return dt.ToString();
			switch (serializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return dt.ToString("s");
				case DateTimeSerializationFormat.JavaConstructor:
					return $"/Date({dt.Ticks / TimeSpan.TicksPerMillisecond})/";
				case DateTimeSerializationFormat.Milliseconds:
					return dt.Ticks / TimeSpan.TicksPerMillisecond;
				case DateTimeSerializationFormat.Custom:
					return dt.ToString(serializer.Options.CustomDateTimeSerializationFormat);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public T Deserialize<T>(JsonValue json, JsonValue root, JsonSerializer serializer)
		{
			if (serializer.Options == null)
				return (T)(object) DateTime.Parse(json.String);
			switch (serializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return (T)(object)DateTime.Parse(json.String);
				case DateTimeSerializationFormat.JavaConstructor:
					return (T)(object)new DateTime(long.Parse(json.String.Substring(6, json.String.Length - 8)) * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Milliseconds:
					return (T)(object)new DateTime((long)json.Number * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Custom:
					return (T)(object)DateTime.ParseExact(json.String, serializer.Options.CustomDateTimeSerializationFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}