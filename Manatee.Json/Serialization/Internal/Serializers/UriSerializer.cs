using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class UriSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContextBase context)
		{
			return context.InferredType == typeof(Uri);
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var uri = context.Source as Uri;

			return uri?.OriginalString;
		}
		public object Deserialize(DeserializationContext context)
		{
			return context.LocalValue == null ? null : new Uri(context.LocalValue.String);
		}
	}
}