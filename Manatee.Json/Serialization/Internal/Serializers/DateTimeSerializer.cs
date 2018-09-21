using System;
using System.Globalization;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DateTimeSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(DateTime);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			var dt = (DateTime) (object) context.Source;
			if (context.RootSerializer.Options == null)
				return dt.ToString();
			switch (context.RootSerializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return dt.ToString("s");
				case DateTimeSerializationFormat.JavaConstructor:
					return $"/Date({dt.Ticks / TimeSpan.TicksPerMillisecond})/";
				case DateTimeSerializationFormat.Milliseconds:
					return dt.Ticks / TimeSpan.TicksPerMillisecond;
				case DateTimeSerializationFormat.Custom:
					return dt.ToString(context.RootSerializer.Options.CustomDateTimeSerializationFormat);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			if (context.RootSerializer.Options == null)
				return (T)(object) DateTime.Parse(context.Source.String);
			switch (context.RootSerializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return (T)(object)DateTime.Parse(context.Source.String);
				case DateTimeSerializationFormat.JavaConstructor:
					return (T)(object)new DateTime(long.Parse(context.Source.String.Substring(6, context.Source.String.Length - 8)) * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Milliseconds:
					return (T)(object)new DateTime((long)context.Source.Number * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Custom:
					return (T)(object)DateTime.ParseExact(context.Source.String, context.RootSerializer.Options.CustomDateTimeSerializationFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}