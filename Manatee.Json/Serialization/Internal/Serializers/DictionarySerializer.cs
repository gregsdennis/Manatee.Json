using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DictionarySerializer : GenericTypeSerializerBase
	{
		public override bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type.GetTypeInfo().IsGenericType &&
			       (type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
			        type.GetGenericTypeDefinition().InheritsFrom(typeof(Dictionary<,>)));
		}

		private static JsonValue _Encode<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer)
		{
			var existingOption = serializer.Options.EncodeDefaultValues;
			var useDefaultValue = existingOption || typeof(TValue).GetTypeInfo().IsValueType;
			if (typeof(TKey) == typeof(string))
			{
				var output = new Dictionary<string, JsonValue>();
				foreach (var kvp in dict)
				{
					var value = _SerializeDefaultValue(serializer, kvp.Value, useDefaultValue, existingOption);
					output.Add((string)(object)kvp.Key, value);
				}

				return output.ToJson();
			}

			if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()))
				return _EncodeEnumKeyDictionary(dict, serializer, useDefaultValue, existingOption);

			return _EncodeDictionary(dict, serializer, useDefaultValue, existingOption);
		}
		private static JsonValue _EncodeDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer, bool useDefaultValue, bool existingOption)
		{
			var array = new JsonValue[dict.Count];
			int i = 0;
			foreach (var item in dict)
			{
				var key = serializer.Serialize(item.Key);
				var value = _SerializeDefaultValue(serializer, item.Value, useDefaultValue, existingOption);
				array[i] = new JsonObject
				{
					{ "Key", key},
					{ "Value", value },
				};
			}
			return new JsonArray(array);
		}
		private static JsonValue _EncodeEnumKeyDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer, bool useDefaultValue, bool existingOption)
		{
			var enumFormat = serializer.Options.EnumSerializationFormat;
			serializer.Options.EnumSerializationFormat = EnumSerializationFormat.AsName;

			var output = new Dictionary<string, JsonValue>();
			foreach (var kvp in dict)
			{
				var key = serializer.Options.SerializationNameTransform(_SerializeDefaultValue(serializer, kvp.Key, true, existingOption).String);
				var value = _SerializeDefaultValue(serializer, kvp.Value, useDefaultValue, existingOption);
				output.Add(key, value);
			}

			serializer.Options.EnumSerializationFormat = enumFormat;

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
		private static JsonValue _SerializeDefaultValue<TValue>(JsonSerializer serializer, TValue item, bool useDefaultValue, bool existingOption)
		{
			serializer.Options.EncodeDefaultValues = useDefaultValue;
			var value = serializer.Serialize(item);
			serializer.Options.EncodeDefaultValues = existingOption;
			return value;
		}
	}
}
