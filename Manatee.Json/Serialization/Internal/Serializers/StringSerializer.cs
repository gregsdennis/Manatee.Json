using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class StringSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type == typeof(string);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			return obj as string;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			return (T) (object) json.String;
		}
	}
}