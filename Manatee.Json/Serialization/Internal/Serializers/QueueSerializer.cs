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
				array.Add(context.RootSerializer.Serialize(queue.ElementAt(i)));
			}
			return array;
		}
		private static Queue<T> _Decode<T>(SerializationContext context)
		{
			var queue = new Queue<T>();
			for (int i = 0; i < context.LocalValue.Array.Count; i++)
			{
				queue.Enqueue(context.RootSerializer.Deserialize<T>(context.LocalValue.Array[i]));
			}
			return queue;
		}
	}
}