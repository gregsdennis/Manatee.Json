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

		private static JsonValue _Encode<T>(SerializationContext<List<T>> context)
		{
			var array = new JsonValue[context.Source.Count];
			for (int ii = 0; ii < array.Length; ++ii)
			{
				array[ii] = context.RootSerializer.Serialize(context.Source[ii]);
			}
			return new JsonArray(array);
		}
		private static List<T> _Decode<T>(SerializationContext<JsonValue> context)
		{
			var array = context.Source.Array;
			var list = new List<T>(array.Count);
			for (int ii = 0; ii < array.Count; ++ii)
			{
				list.Add(context.RootSerializer.Deserialize<T>(array[ii]));
			}
			return list;
		}
	}
}