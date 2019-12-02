using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class NumericSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContextBase context)
		{
			return context.InferredType.IsNumericType();
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var value = Convert.ToDouble(context.Source);
			return value;
		}
		public object Deserialize(DeserializationContext context)
		{
			var value = context.LocalValue.Number;
			return Convert.ChangeType(value, context.InferredType);
		}
	}
}