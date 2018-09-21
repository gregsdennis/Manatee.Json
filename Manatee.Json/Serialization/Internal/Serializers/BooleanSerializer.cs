namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class BooleanSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(bool);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			return (bool) (object)context.Source;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			return (T) (object)context.Source.Boolean;
		}
	}
}