using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class DictionarySerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType &&
			       (context.InferredType.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
			        context.InferredType.GetGenericTypeDefinition().InheritsFrom(typeof(Dictionary<,>)));
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
					var newContext = new SerializationContext
						{
							RootSerializer = context.RootSerializer,
							JsonRoot = context.JsonRoot,
							CurrentLocation = context.CurrentLocation.CloneAndAppend(kvp.Key.ToString()),
							InferredType = kvp.Value?.GetType() ?? typeof(TValue),
							RequestedType = typeof(TValue),
							Source = kvp.Value
						};
					var value = _SerializeDefaultValue(newContext, useDefaultValue, existingOption);
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
				var newContext = new SerializationContext
					{
						RootSerializer = context.RootSerializer,
						JsonRoot = context.JsonRoot,
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString(), "Key"),
						InferredType = item.Key.GetType(),
						RequestedType = typeof(TKey),
						Source = item.Key
					};
				var key = serializer.Serialize(newContext);
				newContext = new SerializationContext
					{
						RootSerializer = context.RootSerializer,
						JsonRoot = context.JsonRoot,
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString(), "Value"),
						InferredType = item.Value.GetType(),
						RequestedType = typeof(TValue),
						Source = item.Value
					};
				var value = _SerializeDefaultValue(newContext, useDefaultValue, existingOption);
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

			var output = new Dictionary<string, JsonValue>();
			int i = 0;
			foreach (var kvp in dict)
			{
				var newContext = new SerializationContext
					{
						RootSerializer = context.RootSerializer,
						JsonRoot = context.JsonRoot,
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString(), "Key"),
						InferredType = kvp.Key.GetType(),
						RequestedType = typeof(TKey),
						Source = kvp.Key
					};
				var key = serializer.Options.SerializationNameTransform(_SerializeDefaultValue(newContext, true, existingOption).String);
				newContext = new SerializationContext
					{
						RootSerializer = context.RootSerializer,
						JsonRoot = context.JsonRoot,
						CurrentLocation = context.CurrentLocation.CloneAndAppend(key),
						InferredType = kvp.Value.GetType(),
						RequestedType = typeof(TValue),
						Source = kvp.Value
					};
				var value = _SerializeDefaultValue(newContext, useDefaultValue, existingOption);
				output.Add(key, value);
				i++;
			}

			serializer.Options.EnumSerializationFormat = enumFormat;

			return output.ToJson();
		}

		private static Dictionary<TKey, TValue> _Decode<TKey, TValue>(SerializationContext context)
		{
			var json = context.LocalValue;

			if (typeof(TKey) == typeof(string))
				return json.Object.ToDictionary(kvp => (TKey)(object)kvp.Key,
												kvp =>
													{
														var newContext = new SerializationContext
															{
																RootSerializer = context.RootSerializer,
																JsonRoot = context.JsonRoot,
																CurrentLocation = context.CurrentLocation.CloneAndAppend(kvp.Key.ToString()),
																InferredType = kvp.Value?.GetType() ?? typeof(TValue),
																RequestedType = typeof(TValue),
																LocalValue = kvp.Value
															};

														return (TValue) context.RootSerializer.Deserialize(newContext);
													});

			if (typeof(Enum).GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo()))
				return _DecodeEnumDictionary<TKey, TValue>(context);

			return json.Array.Select((jv, i) => new {Value = jv, Index = i})
				.ToDictionary(jv =>
					              {
						              var key = jv.Value.Object["Key"];
						              var newContext = new SerializationContext
							              {
								              RootSerializer = context.RootSerializer,
								              JsonRoot = context.JsonRoot,
								              CurrentLocation = context.CurrentLocation.CloneAndAppend(jv.Index.ToString(), "Key"),
								              InferredType = key.GetType() ?? typeof(TValue),
								              RequestedType = typeof(TValue),
								              Source = key
							              };

						              return (TKey) context.RootSerializer.Deserialize(newContext);
					              },
				              jv =>
					              {
						              var key = jv.Value.Object["Key"];
						              var newContext = new SerializationContext
							              {
								              RootSerializer = context.RootSerializer,
								              JsonRoot = context.JsonRoot,
								              CurrentLocation = context.CurrentLocation.CloneAndAppend(jv.Index.ToString(), "Value"),
								              InferredType = key.GetType() ?? typeof(TValue),
								              RequestedType = typeof(TValue),
								              Source = key
							              };
						              return (TValue) context.RootSerializer.Deserialize(newContext);
					              });
		}
		private static Dictionary<TKey, TValue> _DecodeEnumDictionary<TKey, TValue>(SerializationContext context)
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
				var newContext = new SerializationContext
					{
						RootSerializer = context.RootSerializer,
						JsonRoot = context.JsonRoot,
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString(), "Key"),
						InferredType = typeof(TKey),
						RequestedType = typeof(TKey),
						LocalValue = serializer.Options.DeserializationNameTransform(kvp.Key)
				};
				var key = (TKey) serializer.Deserialize(newContext);
				newContext = new SerializationContext
					{
						RootSerializer = context.RootSerializer,
						JsonRoot = context.JsonRoot,
						CurrentLocation = context.CurrentLocation.CloneAndAppend(kvp.Key),
						InferredType = typeof(TValue),
						RequestedType = typeof(TValue),
						LocalValue = kvp.Value
					};
				output.Add(key, (TValue) serializer.Deserialize(newContext));
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
