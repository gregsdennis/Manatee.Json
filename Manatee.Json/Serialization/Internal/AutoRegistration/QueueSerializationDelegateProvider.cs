using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class QueueSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.TypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Queue<>);
		}

		private static JsonValue Encode<T>(Queue<T> queue, JsonSerializer serializer)
		{
			var array = new JsonArray();
			for (int i = 0; i < queue.Count; i++)
			{
				array.Add(serializer.Serialize(queue.ElementAt(i)));
			}
			return array;
		}
		private static Queue<T> Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var queue = new Queue<T>();
			for (int i = 0; i < json.Array.Count; i++)
			{
				queue.Enqueue(serializer.Deserialize<T>(json.Array[i]));
			}
			return queue;
		}
	}
}