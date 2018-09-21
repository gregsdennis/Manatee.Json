using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class TimeSpanSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(TimeSpan);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			var ts = (TimeSpan) (object)context.Source;

			return ts.ToString();
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			return context.Source.Type == JsonValueType.String
				? (T) (object) TimeSpan.Parse(context.Source.String)
				: default(T);
		}
	}
}