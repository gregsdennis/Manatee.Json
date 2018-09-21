using System;
using System.Globalization;

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
		public JsonValue Serialize(SerializationContext context)
		{
			var dt = (DateTime) context.Source;
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
		public object Deserialize(SerializationContext context)
		{
			if (context.RootSerializer.Options == null)
				return DateTime.Parse(context.LocalValue.String);
			switch (context.RootSerializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return DateTime.Parse(context.LocalValue.String);
				case DateTimeSerializationFormat.JavaConstructor:
					return new DateTime(long.Parse(context.LocalValue.String.Substring(6, context.LocalValue.String.Length - 8)) * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Milliseconds:
					return new DateTime((long)context.LocalValue.Number * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Custom:
					return DateTime.ParseExact(context.LocalValue.String, context.RootSerializer.Options.CustomDateTimeSerializationFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}