using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class BooleanSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type == typeof(bool);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			return (bool) (object) obj;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			return (T) (object) json.Boolean;
		}
	}
}