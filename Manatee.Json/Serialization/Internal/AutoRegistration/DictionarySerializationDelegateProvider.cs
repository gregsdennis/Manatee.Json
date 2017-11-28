using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class DictionarySerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.GetTypeInfo().IsGenericType &&
			       (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
			        type.GetGenericTypeDefinition().InheritsFrom(typeof(Dictionary<,>)));
		}

		private static JsonValue _Encode<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer)
		{
			if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()))
				return dict.ToDictionary(kvp =>
					                         {
						                         var encodeDefaults = serializer.Options.EncodeDefaultValues;
						                         serializer.Options.EncodeDefaultValues = true;
						                         var enumFormat = serializer.Options.EnumSerializationFormat;
						                         serializer.Options.EnumSerializationFormat = EnumSerializationFormat.AsName;

						                         var key = serializer.Options.SerializationNameTransform(serializer.Serialize(kvp.Key).String);

						                         serializer.Options.EnumSerializationFormat = enumFormat;
						                         serializer.Options.EncodeDefaultValues = encodeDefaults;

						                         return key;
					                         },
				                         kvp => serializer.Serialize(kvp.Value)).ToJson();

			if (typeof(TKey) == typeof(string))
				return dict.ToDictionary(kvp => (string) (object) kvp.Key, kvp => serializer.Serialize(kvp.Value)).ToJson();

			var array = new JsonArray();
			array.AddRange(dict.Select(item => (JsonValue) new JsonObject
				{
					{"Key", serializer.Serialize(item.Key)},
					{"Value", serializer.Serialize(item.Value)}
				}));
			return array;
		}
		private static Dictionary<TKey, TValue> _Decode<TKey, TValue>(JsonValue json, JsonSerializer serializer)
		{
			if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()))
				return json.Object.ToDictionary(kvp =>
					                                {
						                                var encodeDefaults = serializer.Options.EncodeDefaultValues;
						                                serializer.Options.EncodeDefaultValues = true;
						                                var enumFormat = serializer.Options.EnumSerializationFormat;
						                                serializer.Options.EnumSerializationFormat = EnumSerializationFormat.AsName;

						                                var key = serializer.Deserialize<TKey>(serializer.Options.DeserializationNameTransform(kvp.Key));

						                                serializer.Options.EnumSerializationFormat = enumFormat;
						                                serializer.Options.EncodeDefaultValues = encodeDefaults;
						                                return key;
					                                },
				                                kvp => serializer.Deserialize<TValue>(kvp.Value));

			if (typeof(TKey) == typeof(string))
				return json.Object.ToDictionary(kvp => (TKey) (object) kvp.Key,
				                                kvp => serializer.Deserialize<TValue>(kvp.Value));

			return json.Array.ToDictionary(jv => serializer.Deserialize<TKey>(jv.Object["Key"]),
			                               jv => serializer.Deserialize<TValue>(jv.Object["Value"]));
		}
	}
}
