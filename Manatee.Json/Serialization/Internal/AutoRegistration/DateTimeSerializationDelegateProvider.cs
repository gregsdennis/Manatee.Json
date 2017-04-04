using System;
using System.Globalization;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class DateTimeSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type == typeof(DateTime);
		}

		private static JsonValue Encode(DateTime dt, JsonSerializer serializer)
		{
			if (serializer.Options == null)
				return dt.ToString();
			switch (serializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return dt.ToString("s");
				case DateTimeSerializationFormat.JavaConstructor:
					return $"/Date({dt.Ticks/TimeSpan.TicksPerMillisecond})/";
				case DateTimeSerializationFormat.Milliseconds:
					return dt.Ticks / TimeSpan.TicksPerMillisecond;
				case DateTimeSerializationFormat.Custom:
					return dt.ToString(serializer.Options.CustomDateTimeSerializationFormat);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static DateTime Decode(JsonValue json, JsonSerializer serializer)
		{
			if (serializer.Options == null)
				return DateTime.Parse(json.String);
			switch (serializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return DateTime.Parse(json.String);
				case DateTimeSerializationFormat.JavaConstructor:
					return new DateTime(long.Parse(json.String.Substring(6, json.String.Length - 8)) * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Milliseconds:
					return new DateTime((long)json.Number * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Custom:
					return DateTime.ParseExact(json.String, serializer.Options.CustomDateTimeSerializationFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}