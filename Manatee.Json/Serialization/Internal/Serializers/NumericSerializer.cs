using System;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class NumericSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type.IsNumericType();
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			var value = Convert.ToDouble(obj);
			return value;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			var value = json.Number;
			return (T) Convert.ChangeType(value, typeof (T));
		}
	}
}