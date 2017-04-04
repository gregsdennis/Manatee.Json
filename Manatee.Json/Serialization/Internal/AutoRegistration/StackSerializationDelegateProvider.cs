using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class StackSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.TypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>);
		}

		private static JsonValue Encode<T>(Stack<T> stack, JsonSerializer serializer)
		{
			var array = new JsonArray();
			for (int i = 0; i < stack.Count; i++)
			{
				array.Add(serializer.Serialize(stack.ElementAt(i)));
			}
			return array;
		}
		private static Stack<T> Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var stack = new Stack<T>();
			for (int i = 0; i < json.Array.Count; i++)
			{
				stack.Push(serializer.Deserialize<T>(json.Array[i]));
			}
			return stack;
		}
	}
}