using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class ListSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
		}

		private static JsonValue _Encode<T>(List<T> list, JsonSerializer serializer)
		{
			var array = new JsonArray();
			array.AddRange(list.Select(serializer.Serialize));
			return array;
		}
		private static List<T> _Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var list = new List<T>();
			list.AddRange(json.Array.Select(serializer.Deserialize<T>));
			return list;
		}
	}
}