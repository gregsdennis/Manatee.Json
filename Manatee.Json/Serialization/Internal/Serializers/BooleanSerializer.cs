using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class BooleanSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(bool);
		}
		public JsonValue Serialize<T>(T obj, JsonPointer location, JsonSerializer serializer)
		{
			return (bool) (object) obj;
		}
		public T Deserialize<T>(JsonValue json, JsonValue root, JsonSerializer serializer)
		{
			return (T) (object) json.Boolean;
		}
	}
}