using System;
using Manatee.Json.Internal;

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

		public JsonValue Serialize(SerializationContext context)
		{
			var guid = (Guid) context.Source;
			return guid.ToString();
		}
		public object Deserialize(SerializationContext context)
		{
			return context.LocalValue.Type == JsonValueType.String
				? new Guid(context.LocalValue.String)
				: context.InferredType.Default();
		}
	}
}