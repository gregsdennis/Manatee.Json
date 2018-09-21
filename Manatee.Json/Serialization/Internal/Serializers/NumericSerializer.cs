using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class NumericSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType.IsNumericType();
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			var value = Convert.ToDouble(context.Source);
			return value;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			var value = context.Source.Number;
			return (T) Convert.ChangeType(value, typeof (T));
		}
	}
}