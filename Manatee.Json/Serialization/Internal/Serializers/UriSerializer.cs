using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	[Obsolete("Need to replace this with extensible serializer.")]
	internal class UriSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var uri = obj as Uri;

			return uri?.OriginalString;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			return (T) (object) new Uri(json.String);
		}
	}
}