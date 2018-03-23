using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class GuidSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(Guid);
		}

		private static JsonValue _Encode(Guid guid, JsonSerializer serializer)
		{
			return guid.ToString();
		}
		private static Guid _Decode(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? new Guid(json.String) : default(Guid);
		}
	}
}