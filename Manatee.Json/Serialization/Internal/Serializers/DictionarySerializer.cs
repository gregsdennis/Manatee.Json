using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DictionarySerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContextBase context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType &&
			       context.InferredType.GetGenericTypeDefinition().InheritsFrom(typeof(Dictionary<,>));
		}

		private static JsonValue _Encode<TKey, TValue>(SerializationContext context)
		{
			var dict = (Dictionary<TKey, TValue>) context.Source;
			var existingOption = context.RootSerializer.Options.EncodeDefaultValues;
			var useDefaultValue = existingOption || typeof(TValue).GetTypeInfo().IsValueType;
			if (typeof(TKey) == typeof(string))
			{
				var output = new Dictionary<string, JsonValue>();
				foreach (var kvp in dict)
				{
					context.Push(kvp.Value?.GetType() ?? typeof(TValue), typeof(TValue), kvp.Key.ToString(), kvp.Value);
					var value = _SerializeDefaultValue(context, useDefaultValue, existingOption);
					output.Add((string)(object)kvp.Key, value);
				}

				return output.ToJson();
			}

			if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()))
				return _EncodeEnumKeyDictionary(context, dict, useDefaultValue, existingOption);

			return _EncodeDictionary(context, dict, context.RootSerializer, useDefaultValue, existingOption);
		}
		private static JsonValue _EncodeDictionary<TKey, TValue>(SerializationContext context, Dictionary<TKey, TValue> dict, JsonSerializer serializer, bool useDefaultValue, bool existingOption)
		{
			var array = new JsonValue[dict.Count];
			int i = 0;
			foreach (var item in dict)
			{
				var index = i.ToString();
				
				context.Push(item.Key.GetType(), typeof(TKey), index, item.Key);
				context.Push(item.Key.GetType(), typeof(TKey), "Key", item.Key);
				var key = serializer.Serialize(context);
				context.Pop();
				context.Pop();

				context.Push(item.Value?.GetType() ?? typeof(TValue), typeof(TValue), index, item.Value);
				context.Push(item.Value?.GetType() ?? typeof(TValue), typeof(TValue), "Value", item.Value);
				var value = _SerializeDefaultValue(context, useDefaultValue, existingOption);
				context.Pop();
				context.Pop();

				array[i] = new JsonObject
				{
					{ "Key", key},
					{ "Value", value },
				};
				i++;
			}
			return new JsonArray(array);
		}
		private static JsonValue _EncodeEnumKeyDictionary<TKey, TValue>(SerializationContext context, Dictionary<TKey, TValue> dict, bool useDefaultValue, bool existingOption)
		{
			var serializer = context.RootSerializer;
			var enumFormat = serializer.Options.EnumSerializationFormat;
			serializer.Options.EnumSerializationFormat = EnumSerializationFormat.AsName;

			var output = new JsonObject();
			int i = 0;
			foreach (var kvp in dict)
			{
				var index = i.ToString();
				
				context.Push(kvp.Key.GetType(), typeof(TKey), index, kvp.Key);
				context.Push(kvp.Key.GetType(), typeof(TKey), "Key", kvp.Key);
				var key = serializer.Options.SerializationNameTransform(_SerializeDefaultValue(context, true, existingOption).String);
				context.Pop();
				context.Pop();

				context.Push(kvp.Value?.GetType() ?? typeof(TValue), typeof(TValue), key, kvp.Value);
				var value = _SerializeDefaultValue(context, useDefaultValue, existingOption);
				context.Pop();

				output.Add(key, value);
				i++;
			}

			serializer.Options.EnumSerializationFormat = enumFormat;

			return output;
		}

		private static Dictionary<TKey, TValue> _Decode<TKey, TValue>(DeserializationContext context)
		{
			var json = context.LocalValue;

			if (typeof(TKey) == typeof(string))
				return json.Object.ToDictionary(kvp => (TKey)(object)kvp.Key,
												kvp =>
													{
														context.Push(typeof(TValue), kvp.Key.ToString(), kvp.Value);
														var value = (TValue) context.RootSerializer.Deserialize(context);
														context.Pop();
														return value;
													});

			if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()))
				return _DecodeEnumDictionary<TKey, TValue>(context);

			return json.Array.Select((jv, i) => new {Value = jv, Index = i})
				.ToDictionary(jv =>
					              {
						              var key = jv.Value.Object["Key"];
									  context.Push(typeof(TKey), jv.Index.ToString(), key);
									  context.Push(typeof(TKey), "Key", key);
						              var obj = (TKey) context.RootSerializer.Deserialize(context);
									  context.Pop();
									  context.Pop();
									  return obj;
					              },
				              jv =>
					              {
						              var value = jv.Value.Object["Value"];
									  context.Push(typeof(TValue), jv.Index.ToString(), value);
									  context.Push(typeof(TValue), "Value", value);
						              var obj = (TValue) context.RootSerializer.Deserialize(context);
									  context.Pop();
									  context.Pop();
									  return obj;
					              });
		}
		private static Dictionary<TKey, TValue> _DecodeEnumDictionary<TKey, TValue>(DeserializationContext context)
		{
			var serializer = context.RootSerializer;
			var encodeDefaults = serializer.Options.EncodeDefaultValues;
			serializer.Options.EncodeDefaultValues = true;
			var enumFormat = serializer.Options.EnumSerializationFormat;
			serializer.Options.EnumSerializationFormat = EnumSerializationFormat.AsName;

			var output = new Dictionary<TKey, TValue>();
			var i = 0;
			foreach (var kvp in context.LocalValue.Object)
			{
				var transformed = serializer.Options.DeserializationNameTransform(kvp.Key);
				context.Push(typeof(TKey), i.ToString(), transformed);
				context.Push(typeof(TKey), "Key", transformed);
				var key = (TKey) serializer.Deserialize(context);
				context.Pop();
				context.Pop();

				context.Push(typeof(TValue), kvp.Key, kvp.Value);
				output.Add(key, (TValue) serializer.Deserialize(context));
				context.Pop();
				i++;
			}

			serializer.Options.EnumSerializationFormat = enumFormat;
			serializer.Options.EncodeDefaultValues = encodeDefaults;

			return output;
		}

		private static JsonValue _SerializeDefaultValue(SerializationContext context, bool useDefaultValue, bool existingOption)
		{
			context.RootSerializer.Options.EncodeDefaultValues = useDefaultValue;
			var value = context.RootSerializer.Serialize(context);
			context.RootSerializer.Options.EncodeDefaultValues = existingOption;
			return value;
		}
	}
}
