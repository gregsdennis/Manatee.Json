using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DefaultValueSerializer : IChainedSerializer
	{
		public static DefaultValueSerializer Instance { get; } = new DefaultValueSerializer();

		private DefaultValueSerializer() { }

		public JsonValue TrySerialize(ISerializer serializer, SerializationContext context)
		{
			Log.Serialization("Checking for default value");
			if (Equals(context.Source, context.RequestedType.Default()) &&
			    !context.RootSerializer.Options.EncodeDefaultValues)
			{
				Log.Serialization($"Value was default for {context.InferredType.CSharpName()}; returning JSON null");
				return JsonValue.Null;
			}

			if (ReferencingSerializer.Handles(serializer, context.InferredType))
				return ReferencingSerializer.Instance.TrySerialize(serializer, context);

			return serializer.Serialize(context);
		}
		public object? TryDeserialize(ISerializer serializer, DeserializationContext context)
		{
			if (context.LocalValue.Type == JsonValueType.Null)
			{
				Log.Serialization($"Found JSON null; returning default for {context.InferredType.CSharpName()}");
				return context.InferredType.Default();
			}

			if (ReferencingSerializer.Handles(serializer, context.InferredType))
				return ReferencingSerializer.Instance.TryDeserialize(serializer, context);

			return serializer.Deserialize(context);
		}
	}
}