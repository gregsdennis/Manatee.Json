using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class StackSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType && 
			       context.InferredType.GetGenericTypeDefinition() == typeof(Stack<>);
		}

		private static JsonValue _Encode<T>(SerializationContext context)
		{
			var stack = (Stack<T>) context.Source;
			var values = new JsonValue[stack.Count];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = context.RootSerializer.Serialize(stack.ElementAt(i));
			}
			return new JsonArray(values);
		}
		private static Stack<T> _Decode<T>(SerializationContext context)
		{
			var array = context.LocalValue.Array;
			var values = new T[array.Count];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = context.RootSerializer.Deserialize<T>(array[i]);
			}
			return new Stack<T>(values);
		}
	}
}