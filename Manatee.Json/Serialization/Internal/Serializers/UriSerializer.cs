using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class UriSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(Uri);
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var uri = context.Source as Uri;

			return uri?.OriginalString;
		}
		public object Deserialize(SerializationContext context)
		{
			return context.LocalValue == null ? null : new Uri(context.LocalValue.String);
		}
	}
}