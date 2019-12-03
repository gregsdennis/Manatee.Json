using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DefaultValueSerializer : IChainedSerializer
	{
		public static DefaultValueSerializer Instance { get; } = new DefaultValueSerializer();

		private DefaultValueSerializer() { }

		public JsonValue TrySerialize(ISerializer serializer, SerializationContext context)
		{
			if (Equals(context.Source, context.RequestedType.Default()) &&
			    !context.RootSerializer.Options.EncodeDefaultValues)
				return JsonValue.Null;

			if (ReferencingSerializer.Instance.Handles(serializer, context.InferredType))
				return ReferencingSerializer.Instance.TrySerialize(serializer, context);

			return serializer.Serialize(context);
		}
		public object? TryDeserialize(ISerializer serializer, DeserializationContext context)
		{
			if (context.LocalValue.Type == JsonValueType.Null)
				return context.InferredType.Default();

			if (ReferencingSerializer.Instance.Handles(serializer, context.InferredType))
				return ReferencingSerializer.Instance.TryDeserialize(serializer, context);

			return serializer.Deserialize(context);
		}
	}
}