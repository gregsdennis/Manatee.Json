using System;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class UriSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type == typeof(Uri);
		}

		private static JsonValue _Encode(Uri uri, JsonSerializer serializer)
		{
			return uri.OriginalString;
		}
		private static Uri _Decode(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? new Uri(json.String) : null;
		}
	}
}