namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class StringSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(string);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			return context.Source as string;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			return (T) (object)context.Source.String;
		}
	}
}