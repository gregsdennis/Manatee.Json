using System;
using JetBrains.Annotations;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	[UsedImplicitly]
	internal class GuidSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContextBase context)
		{
			return context.InferredType == typeof(Guid);
		}

		public JsonValue Serialize(SerializationContext context)
		{
			var guid = (Guid) context.Source!;
			return guid.ToString();
		}
		public object Deserialize(DeserializationContext context)
		{
			return context.LocalValue.Type == JsonValueType.String
				? new Guid(context.LocalValue.String)
				: Guid.Empty;
		}
	}
}