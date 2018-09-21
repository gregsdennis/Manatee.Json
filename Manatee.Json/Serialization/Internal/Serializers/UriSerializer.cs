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
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			var uri = context.Source as Uri;

			return uri?.OriginalString;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			return (T) (object) new Uri(context.Source.String);
		}
	}
}