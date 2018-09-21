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
		public JsonValue Serialize(SerializationContext context)
		{
			var schema = (JsonSchema) context.Source;
			return schema.ToJson(context.RootSerializer);
		}
		public object Deserialize(SerializationContext context)
		{
			var schema = new JsonSchema();
			schema.FromJson(context.LocalValue, context.RootSerializer);
			return schema;
		}
	}
}