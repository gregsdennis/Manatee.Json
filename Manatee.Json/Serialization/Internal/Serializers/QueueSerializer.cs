using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class QueueSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType &&
			       context.InferredType.GetGenericTypeDefinition() == typeof(Queue<>);
		}

		private static JsonValue _Encode<T>(SerializationContext context)
		{
			var queue = (Queue<T>) context.Source;
			var array = new JsonArray();
			for (int i = 0; i < queue.Count; i++)
			{
				var element = queue.ElementAt(i);
				var newContext = new SerializationContext(context)
				{
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
						InferredType = element?.GetType() ?? typeof(T),
						RequestedType = typeof(T),
						Source = element
					};
				array.Add(context.RootSerializer.Serialize(newContext));
			}
			return array;
		}
		private static Queue<T> _Decode<T>(SerializationContext context)
		{
			var queue = new Queue<T>();
			for (int i = 0; i < context.LocalValue.Array.Count; i++)
			{
				var newContext = new SerializationContext(context)
				{
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
						InferredType = typeof(T),
						RequestedType = typeof(T),
						LocalValue = context.LocalValue.Array[i]
					};
				queue.Enqueue((T)context.RootSerializer.Deserialize(newContext));
			}
			return queue;
		}
	}
}