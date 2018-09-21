using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class UriSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type == typeof(Uri);
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			var uri = obj as Uri;

			return uri?.OriginalString;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			return (T) (object) new Uri(json.String);
		}
	}
}