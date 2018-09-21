using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class GuidSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(Guid);
		}

		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			var guid = (Guid) (object)context.Source;
			return guid.ToString();
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			return context.Source.Type == JsonValueType.String
				? (T) (object) new Guid(context.Source.String)
				: default(T);
		}
	}
}