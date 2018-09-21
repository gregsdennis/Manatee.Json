using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class JsonSerializableSerializer : IPrioritizedSerializer
	{
		public int Priority => -9;

		public bool ShouldMaintainReferences => true;

		public bool Handles(SerializationContext context)
		{
			return typeof(IJsonSerializable).GetTypeInfo().IsAssignableFrom(context.InferredType.GetTypeInfo());
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			var serializable = (IJsonSerializable)context.Source;
			return serializable.ToJson(context.RootSerializer);
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			var value = (IJsonSerializable)context.RootSerializer.AbstractionMap.CreateInstance<T>(context.Source, context.RootSerializer.Options.Resolver);
			value.FromJson(context.Source, context.RootSerializer);
			return (T) value;
		}
	}
}