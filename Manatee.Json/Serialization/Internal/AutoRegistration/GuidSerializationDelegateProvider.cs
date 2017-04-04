using System;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class GuidSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type == typeof(Guid);
		}

		private static JsonValue Encode(Guid guid, JsonSerializer serializer)
		{
			return guid.ToString();
		}
		private static Guid Decode(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? new Guid(json.String) : default(Guid);
		}
	}
}