using System.Collections.Generic;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ListSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType && 
			       context.InferredType.GetGenericTypeDefinition() == typeof(List<>);
		}

		private static JsonValue _Encode<T>(SerializationContext context)
		{
			var list = (List<T>) context.Source;
			var array = new JsonValue[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				var newContext = new SerializationContext(context)
				{
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
						InferredType = list[i]?.GetType() ?? typeof(T),
						RequestedType = typeof(T),
						Source = list[i]
					};

				array[i] = context.RootSerializer.Serialize(newContext);
			}
			return new JsonArray(array);
		}
		private static List<T> _Decode<T>(SerializationContext context)
		{
			var array = context.LocalValue.Array;
			var list = new List<T>(array.Count);
			for (int i = 0; i < array.Count; i++)
			{
				var newContext = new SerializationContext(context)
				{
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
						InferredType = typeof(T),
						RequestedType = typeof(T),
						LocalValue = array[i]
					};
				list.Add((T) context.RootSerializer.Deserialize(newContext));
			}
			return list;
		}
	}
}