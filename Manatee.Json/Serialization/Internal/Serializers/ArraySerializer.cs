using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ArraySerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return type.IsArray;
		}

		protected override Type[] GetTypeArguments(Type type)
		{
			return new[] { type.GetElementType() };
		}

		private static JsonValue _Encode<T>(T[] array, JsonSerializer serializer)
		{
			var values = new JsonValue[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				values[i] = serializer.Serialize(array[i]);
			}
			return new JsonArray(values);
		}
		private static T[] _Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var array = json.Array;
			var values = new T[array.Count];
			for (int i = 0; i < array.Count; i++)
			{
				values[i] = serializer.Deserialize<T>(array[i]);
			}
			return values;
		}
	}
}