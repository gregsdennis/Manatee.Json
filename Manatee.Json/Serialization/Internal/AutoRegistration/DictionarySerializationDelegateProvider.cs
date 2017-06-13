using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class DictionarySerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.TypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
		}

		private static JsonValue Encode<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer)
		{
			var array = new JsonArray();
			array.AddRange(dict.Select(item => (JsonValue)(new JsonObject
				{
					{"Key", serializer.Serialize(item.Key)},
					{"Value", serializer.Serialize(item.Value)}
				})));
			return array;
		}
		private static Dictionary<TKey, TValue> Decode<TKey, TValue>(JsonValue json, JsonSerializer serializer)
		{
			return json.Array.ToDictionary(jv => serializer.Deserialize<TKey>(jv.Object["Key"]),
										   jv => serializer.Deserialize<TValue>(jv.Object["Value"]));
		}
	}
}