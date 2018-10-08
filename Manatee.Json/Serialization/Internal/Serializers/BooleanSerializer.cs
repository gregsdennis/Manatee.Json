namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class BooleanSerializer : IPrioritizedSerializer
	{
		public int Priority => 1;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(bool);
		}
		public JsonValue Serialize(SerializationContext context)
		{
			return (bool) context.Source;
		}
		public object Deserialize(SerializationContext context)
		{
			return context.LocalValue.Boolean;
		}
	}
}