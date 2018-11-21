using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ListSerializer : GenericTypeSerializerBase
	{
		public override int Priority => 3;

		public override bool Handles(SerializationContext context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType &&
			       context.InferredType.InheritsFrom(typeof(IEnumerable));
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

		protected override void PrepSource(SerializationContext context)
		{
			var sourceType = context.Source.GetType();
			if (sourceType.GetTypeInfo().IsGenericType &&
			    sourceType.GetGenericTypeDefinition().InheritsFrom(typeof(List<>))) return;

			context.Source = (context.Source as IEnumerable).Cast<object>().ToList();
		}
	}
}