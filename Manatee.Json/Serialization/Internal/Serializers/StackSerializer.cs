using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class StackSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContextBase context)
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
				var element = stack.ElementAt(i);
				context.Push(element?.GetType() ?? typeof(T), typeof(T), i.ToString(), element);
				values[i] = context.RootSerializer.Serialize(context);
				context.Pop();
			}
			return new JsonArray(values);
		}
		private static Stack<T> _Decode<T>(DeserializationContext context)
		{
			var array = context.LocalValue.Array;
			var values = new T[array.Count];
			for (int i = 0; i < values.Length; i++)
			{
				context.Push(typeof(T), i.ToString(), array[i]);
				values[i] = (T)context.RootSerializer.Deserialize(context);
				context.Pop();
			}
			return new Stack<T>(values);
		}
	}
}