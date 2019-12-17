using JetBrains.Annotations;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	[UsedImplicitly]
	internal class StringSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContextBase context)
		{
			return context.InferredType == typeof(string);
		}
		public JsonValue Serialize(SerializationContext context)
		{
			return context.Source as string;
		}
		public object Deserialize(DeserializationContext context)
		{
			return context.LocalValue.String;
		}
	}
}