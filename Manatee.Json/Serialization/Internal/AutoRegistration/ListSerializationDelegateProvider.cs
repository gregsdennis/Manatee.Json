using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class ListSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.TypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
		}

		private static JsonValue Encode<T>(List<T> list, JsonSerializer serializer)
		{
			var array = new JsonArray();
			array.AddRange(list.Select(serializer.Serialize));
			return array;
		}
		private static List<T> Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var list = new List<T>();
			list.AddRange(json.Array.Select(serializer.Deserialize<T>));
			return list;
		}
	}
}