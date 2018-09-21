using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SchemaSerializer : IPrioritizedSerializer
	{
		public bool ShouldMaintainReferences => false;
		public int Priority => -10;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(JsonSchema);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			var schema = (JsonSchema) (object)context.Source;
			return schema.ToJson(context.RootSerializer);
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			var schema = new JsonSchema();
			schema.FromJson(context.Source, context.RootSerializer);
			return (T) (object) schema;
		}
	}
}