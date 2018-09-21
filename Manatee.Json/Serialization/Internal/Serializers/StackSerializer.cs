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

		private static JsonValue _Encode<T>(SerializationContext<Stack<T>> context)
		{
			var values = new JsonValue[context.Source.Count];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = context.RootSerializer.Serialize(context.Source.ElementAt(i));
			}
			return new JsonArray(values);
		}
		private static Stack<T> _Decode<T>(SerializationContext<JsonValue> context)
		{
			var array = context.Source.Array;
			var values = new T[array.Count];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = context.RootSerializer.Deserialize<T>(array[i]);
			}
			return new Stack<T>(values);
		}
	}
}