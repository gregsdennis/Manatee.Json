using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class NullableSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.TypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		private static JsonValue Encode<T>(T? nullable, JsonSerializer serializer)
			where T : struct
		{
			if (!nullable.HasValue) return JsonValue.Null;
			var encodeDefaultValues = serializer.Options.EncodeDefaultValues;
			serializer.Options.EncodeDefaultValues = Equals(nullable.Value, default (T));
			var json = serializer.Serialize(nullable.Value);
			serializer.Options.EncodeDefaultValues = encodeDefaultValues;
			return json;
		}
		private static T? Decode<T>(JsonValue json, JsonSerializer serializer)
			where T : struct
		{
			if (json == JsonValue.Null)
				return null;
			T? nullable = serializer.Deserialize<T>(json);
			return nullable;
		}
	}
}