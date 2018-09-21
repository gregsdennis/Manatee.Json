using System;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaSerializer : IPrioritizedSerializer
	{
		public bool ShouldMaintainReferences => false;
		public int Priority => -10;

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type == typeof(JsonSchema);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			var schema = (JsonSchema) (object) obj;
			return schema.ToJson(serializer);
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			var schema = new JsonSchema();
			schema.FromJson(json, serializer);
			return (T) (object) schema;
		}
	}
}