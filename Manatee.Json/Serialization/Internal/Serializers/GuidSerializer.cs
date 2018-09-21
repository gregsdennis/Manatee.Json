using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class GuidSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type == typeof(Guid);
		}

		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			var guid = (Guid) (object) obj;
			return guid.ToString();
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			return json.Type == JsonValueType.String ? (T) (object) new Guid(json.String) : default(T);
		}
	}
}