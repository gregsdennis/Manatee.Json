using System;
using System.Collections.Generic;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ListSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
		}

		private static JsonValue _Encode<T>(List<T> list, JsonSerializer serializer)
		{
			var array = new JsonValue[list.Count];
			for (int ii = 0; ii < array.Length; ++ii)
			{
				array[ii] = serializer.Serialize(list[ii]);
			}
			return new JsonArray(array);
		}
		private static List<T> _Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var array = json.Array;
			var list = new List<T>(array.Count);
			for (int ii = 0; ii < array.Count; ++ii)
			{
				list.Add(serializer.Deserialize<T>(array[ii]));
			}
			return list;
		}
	}
}