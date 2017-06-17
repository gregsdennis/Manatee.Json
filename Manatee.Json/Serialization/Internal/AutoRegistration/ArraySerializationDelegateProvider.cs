using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class ArraySerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.IsArray;
		}

		protected override Type[] GetTypeArguments(Type type)
		{
			return new[] { type.GetElementType() };
		}

		private static JsonValue _Encode<T>(T[] array, JsonSerializer serializer)
		{
			var json = new JsonArray();
			json.AddRange(array.Select(serializer.Serialize));
			return json;
		}
		private static T[] _Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var list = new List<T>();
			list.AddRange(json.Array.Select(serializer.Deserialize<T>));
			return list.ToArray();
		}
	}
}