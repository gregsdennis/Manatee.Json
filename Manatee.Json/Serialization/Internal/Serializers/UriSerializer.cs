using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class UriSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(Uri);
		}
		public JsonValue Serialize<T>(T obj, JsonPointer location, JsonSerializer serializer)
		{
			var uri = obj as Uri;

			return uri?.OriginalString;
		}
		public T Deserialize<T>(JsonValue json, JsonValue root, JsonSerializer serializer)
		{
			return (T) (object) new Uri(json.String);
		}
	}
}