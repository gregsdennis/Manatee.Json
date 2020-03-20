using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class TimeSpanSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContextBase context)
		{
			return context.InferredType == typeof(TimeSpan);
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var ts = (TimeSpan) context.Source!;

			return ts.ToString();
		}
		public object Deserialize(DeserializationContext context)
		{
			return context.LocalValue.Type == JsonValueType.String
				? TimeSpan.Parse(context.LocalValue.String)
				: default;
		}
	}
}