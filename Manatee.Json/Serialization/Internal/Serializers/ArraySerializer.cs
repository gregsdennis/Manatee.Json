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

		private static JsonValue _Encode<T>(SerializationContext<T[]> context)
		{
			var values = new JsonValue[context.Source.Length];
			for (int i = 0; i < context.Source.Length; i++)
			{
				values[i] = context.RootSerializer.Serialize(context.Source[i]);
			}
			return new JsonArray(values);
		}
		private static T[] _Decode<T>(SerializationContext<JsonValue> context)
		{
			var array = context.Source.Array;
			var values = new T[array.Count];
			for (int i = 0; i < array.Count; i++)
			{
				values[i] = context.RootSerializer.Deserialize<T>(array[i]);
			}
			return values;
		}
	}
}