using System;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaSerializer : IPrioritizedSerializer
	{
		public bool ShouldMaintainReferences => false;
		public int Priority => -10;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(JsonSchema);
		}
		public JsonValue Serialize<T>(T obj, JsonPointer location, JsonSerializer serializer)
		{
			var schema = (JsonSchema) (object) obj;
			return schema.ToJson(serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonValue root, JsonSerializer serializer)
		{
			var schema = new JsonSchema();
			schema.FromJson(json, serializer);
			return (T) (object) schema;
		}
	}
}