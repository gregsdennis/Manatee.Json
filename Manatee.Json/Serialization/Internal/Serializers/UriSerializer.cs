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
			var uri = (Uri) context.Source!;

			return uri.OriginalString;
		}
		public object Deserialize(DeserializationContext context)
		{
			return new Uri(context.LocalValue.String);
		}
	}
}