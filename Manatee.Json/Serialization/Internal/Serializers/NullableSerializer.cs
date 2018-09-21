using System;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class NullableSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		private static JsonValue _Encode<T>(T? nullable, JsonSerializer serializer)
			where T : struct
		{
			if (!nullable.HasValue) return JsonValue.Null;
			var encodeDefaultValues = serializer.Options.EncodeDefaultValues;
			serializer.Options.EncodeDefaultValues = Equals(nullable.Value, default (T));
			var json = serializer.Serialize(nullable.Value);
			serializer.Options.EncodeDefaultValues = encodeDefaultValues;
			return json;
		}
		private static T? _Decode<T>(JsonValue json, JsonSerializer serializer)
			where T : struct
		{
			if (json == JsonValue.Null)
				return null;
			T? nullable = serializer.Deserialize<T>(json);
			return nullable;
		}
	}
}