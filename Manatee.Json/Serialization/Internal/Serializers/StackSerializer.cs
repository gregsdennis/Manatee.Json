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
				var element = stack.ElementAt(i);
				var newContext = new SerializationContext(context)
				{
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
						InferredType = element?.GetType() ?? typeof(T),
						RequestedType = typeof(T),
						Source = element
				};

				values[i] = context.RootSerializer.Serialize(newContext);
			}
			return new JsonArray(values);
		}
		private static Stack<T> _Decode<T>(SerializationContext context)
		{
			var array = context.LocalValue.Array;
			var values = new T[array.Count];
			for (int i = 0; i < values.Length; i++)
			{
				var newContext = new SerializationContext(context)
				{
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
						InferredType = typeof(T),
						RequestedType = typeof(T),
						LocalValue = array[i]
					};
				values[i] = (T)context.RootSerializer.Deserialize(newContext);
			}
			return new Stack<T>(values);
		}
	}
}