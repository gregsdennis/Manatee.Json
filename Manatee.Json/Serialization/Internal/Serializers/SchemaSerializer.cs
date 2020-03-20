using JetBrains.Annotations;
using Manatee.Json.Schema;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	[UsedImplicitly]
	internal class SchemaSerializer : IPrioritizedSerializer
	{
		public bool ShouldMaintainReferences => false;
		public int Priority => 1;

		public bool Handles(SerializationContextBase context)
		{
			return context.InferredType == typeof(JsonSchema);
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var schema = (JsonSchema) context.Source!;
			return schema.ToJson(context.RootSerializer);
		}
		public object Deserialize(DeserializationContext context)
		{
			var schema = new JsonSchema();
			schema.FromJson(context.LocalValue, context.RootSerializer);
			return schema;
		}
	}
}