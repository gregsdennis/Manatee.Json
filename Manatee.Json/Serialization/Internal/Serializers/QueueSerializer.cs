using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	[UsedImplicitly]
	internal class QueueSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContextBase context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType &&
			       context.InferredType.GetGenericTypeDefinition() == typeof(Queue<>);
		}

		[UsedImplicitly]
		private static JsonValue _Encode<T>(SerializationContext context)
		{
			var queue = (Queue<T>) context.Source!;
			var array = new JsonArray();
			for (int i = 0; i < queue.Count; i++)
			{
				var element = queue.ElementAt(i);
				context.Push(element?.GetType() ?? typeof(T), typeof(T), i.ToString(), element);
				array.Add(context.RootSerializer.Serialize(context));
				context.Pop();
			}
			return array;
		}
		[UsedImplicitly]
		private static Queue<T> _Decode<T>(DeserializationContext context)
		{
			var queue = new Queue<T>();
			for (int i = 0; i < context.LocalValue.Array.Count; i++)
			{
				context.Push(typeof(T), i.ToString(), context.LocalValue.Array[i]);
				queue.Enqueue((T)context.RootSerializer.Deserialize(context));
				context.Pop();
			}
			return queue;
		}
	}
}