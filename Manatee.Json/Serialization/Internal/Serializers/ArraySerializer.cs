using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ArraySerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context)
		{
			return context.InferredType.IsArray;
		}

		protected override Type[] GetTypeArguments(Type type)
		{
			return new[] { type.GetElementType() };
		}

		private static JsonValue _Encode<T>(SerializationContext context)
		{
			var array = (T[]) context.Source;
			var values = new JsonValue[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				values[i] = context.RootSerializer.Serialize(array[i]);
			}
			return new JsonArray(values);
		}
		private static T[] _Decode<T>(SerializationContext context)
		{
			var array = context.LocalValue.Array;
			var values = new T[array.Count];
			for (int i = 0; i < array.Count; i++)
			{
				values[i] = context.RootSerializer.Deserialize<T>(array[i]);
			}
			return values;
		}
	}
}