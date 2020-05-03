using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	[UsedImplicitly]
	internal class DateTimeSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContextBase context)
		{
			return context.InferredType == typeof(DateTime);
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var dt = (DateTime) context.Source!;
			var options = context.RootSerializer.Options;
			switch (options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return dt.ToString("O", CultureInfo.InvariantCulture);
				case DateTimeSerializationFormat.JavaConstructor:
					return $"/Date({dt.Ticks / TimeSpan.TicksPerMillisecond})/";
				case DateTimeSerializationFormat.Milliseconds:
					return dt.Ticks / TimeSpan.TicksPerMillisecond;
				case DateTimeSerializationFormat.Custom:
					if (string.IsNullOrWhiteSpace(options.CustomDateTimeSerializationFormat))
						throw new ArgumentNullException($"{nameof(JsonSerializerOptions)}.{nameof(JsonSerializerOptions.CustomDateTimeSerializationFormat)}");
					return dt.ToString(options.CustomDateTimeSerializationFormat);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public object Deserialize(DeserializationContext context)
		{
			var options = context.RootSerializer.Options;
			switch (options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return DateTime.Parse(context.LocalValue.String, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
				case DateTimeSerializationFormat.JavaConstructor:
					return new DateTime(long.Parse(context.LocalValue.String.Substring(6, context.LocalValue.String.Length - 8)) * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Milliseconds:
					return new DateTime((long)context.LocalValue.Number * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Custom:
					if (string.IsNullOrWhiteSpace(options.CustomDateTimeSerializationFormat))
						throw new ArgumentNullException($"{nameof(JsonSerializerOptions)}.{nameof(JsonSerializerOptions.CustomDateTimeSerializationFormat)}");
					return DateTime.ParseExact(context.LocalValue.String, options.CustomDateTimeSerializationFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}