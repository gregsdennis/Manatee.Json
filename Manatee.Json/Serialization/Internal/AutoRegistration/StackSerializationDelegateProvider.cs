using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class StackSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>);
		}

		private static JsonValue _Encode<T>(Stack<T> stack, JsonSerializer serializer)
		{
			var values = new JsonValue[stack.Count];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = serializer.Serialize(stack.ElementAt(i));
			}
			return new JsonArray(values);
		}
		private static Stack<T> _Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var array = json.Array;
			var values = new T[array.Count];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = serializer.Deserialize<T>(array[i]);
			}
			return new Stack<T>(values);
		}
	}
}