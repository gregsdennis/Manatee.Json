using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DefaultValueSerializer : ISerializer
	{
		private readonly ISerializer _innerSerializer;

		public bool ShouldMaintainReferences => _innerSerializer.ShouldMaintainReferences;

		public DefaultValueSerializer(ISerializer innerSerializer)
		{
			_innerSerializer = innerSerializer;
		}

		public bool Handles(SerializationContextBase context)
		{
			return true;
		}
		public JsonValue Serialize(SerializationContext context)
		{
			if (Equals(context.Source, context.RequestedType.Default()) &&
			    !context.RootSerializer.Options.EncodeDefaultValues)
				return JsonValue.Null;

			return _innerSerializer.Serialize(context);
		}
		public object Deserialize(DeserializationContext context)
		{
			return context.LocalValue.Type == JsonValueType.Null
				       ? context.InferredType.Default()
				       : _innerSerializer.Deserialize(context);
		}
	}
}