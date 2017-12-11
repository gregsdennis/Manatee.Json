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
			var values = new JsonValue[array.Length];
			for (int ii = 0; ii < array.Length; ++ii)
			{
				values[ii] = serializer.Serialize(array[ii]);
			}
			return new JsonArray(values);
		}
		private static T[] _Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var array = json.Array;
			var values = new T[array.Count];
			for (int ii = 0; ii < array.Count; ++ii)
			{
				values[ii] = serializer.Deserialize<T>(array[ii]);
			}
			return values;
		}
	}
}