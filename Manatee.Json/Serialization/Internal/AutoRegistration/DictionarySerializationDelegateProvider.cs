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
			if (typeof(TKey) == typeof(string))
			{
				var output = new Dictionary<string, JsonValue>();
				foreach (var kvp in dict)
				{
					output.Add((string)(object)kvp.Key, serializer.Serialize(kvp.Value));
				}

				return output.ToJson();
			}

			if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()))
				return _EncodeEnumKeyDictionary(dict, serializer);

			return _EncodeDictionary(dict, serializer);
		}
		private static JsonValue _EncodeDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer)
		{
			var array = new JsonValue[dict.Count];
			int ii = 0;
			foreach (var item in dict)
			{
				array[ii] = new JsonObject
				{
					{ "Key", serializer.Serialize(item.Key) },
					{ "Value", serializer.Serialize(item.Value) },
				};
			}
			return new JsonArray(array);
		}
		private static JsonValue _EncodeEnumKeyDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer)
		{
			var encodeDefaults = serializer.Options.EncodeDefaultValues;
			serializer.Options.EncodeDefaultValues = true;
			var enumFormat = serializer.Options.EnumSerializationFormat;
			serializer.Options.EnumSerializationFormat = EnumSerializationFormat.AsName;

			var output = new Dictionary<string, JsonValue>();
			foreach (var kvp in dict)
			{
				var key = serializer.Options.SerializationNameTransform(serializer.Serialize(kvp.Key).String);
				output.Add(key, serializer.Serialize(kvp.Value));
			}

			serializer.Options.EnumSerializationFormat = enumFormat;
			serializer.Options.EncodeDefaultValues = encodeDefaults;

			return output.ToJson();
		}
		private static Dictionary<TKey, TValue> _Decode<TKey, TValue>(JsonValue json, JsonSerializer serializer)
		{
			if (typeof(TKey) == typeof(string))
				return json.Object.ToDictionary(kvp => (TKey)(object)kvp.Key,
												kvp => serializer.Deserialize<TValue>(kvp.Value));

			if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()))
				return _DecodeEnumDictionary<TKey, TValue>(json, serializer);

			return json.Array.ToDictionary(jv => serializer.Deserialize<TKey>(jv.Object["Key"]),
			                               jv => serializer.Deserialize<TValue>(jv.Object["Value"]));
		}
		private static Dictionary<TKey, TValue> _DecodeEnumDictionary<TKey, TValue>(JsonValue json, JsonSerializer serializer)
		{
			var encodeDefaults = serializer.Options.EncodeDefaultValues;
			serializer.Options.EncodeDefaultValues = true;
			var enumFormat = serializer.Options.EnumSerializationFormat;
			serializer.Options.EnumSerializationFormat = EnumSerializationFormat.AsName;

			var output = new Dictionary<TKey, TValue>();
			foreach (var kvp in json.Object)
			{
				var key = serializer.Deserialize<TKey>(serializer.Options.DeserializationNameTransform(kvp.Key));
				output.Add(key, serializer.Deserialize<TValue>(kvp.Value));
			}

			serializer.Options.EnumSerializationFormat = enumFormat;
			serializer.Options.EncodeDefaultValues = encodeDefaults;

			return output;
		}
	}
}
