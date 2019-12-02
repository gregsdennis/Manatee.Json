using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class JsonSerializableSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => true;

		public bool Handles(SerializationContextBase context)
		{
			return typeof(IJsonSerializable).GetTypeInfo().IsAssignableFrom(context.InferredType.GetTypeInfo());
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var serializable = (IJsonSerializable)context.Source;
			return serializable.ToJson(context.RootSerializer);
		}
		public object Deserialize(DeserializationContext context)
		{
			var value = (IJsonSerializable)context.RootSerializer.AbstractionMap.CreateInstance(context);
			value.FromJson(context.LocalValue, context.RootSerializer);
			return value;
		}
	}
}