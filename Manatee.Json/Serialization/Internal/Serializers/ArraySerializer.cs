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
				var newContext = new SerializationContext(context)
					{
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
						InferredType = array[i]?.GetType() ?? typeof(T),
						RequestedType = typeof(T),
						Source = array[i]
					};

				values[i] = context.RootSerializer.Serialize(newContext);
			}
			return new JsonArray(values);
		}
		private static T[] _Decode<T>(SerializationContext context)
		{
			var array = context.LocalValue.Array;
			var values = new T[array.Count];
			for (int i = 0; i < array.Count; i++)
			{
				var newContext = new SerializationContext(context)
				{
						CurrentLocation = context.CurrentLocation.CloneAndAppend(i.ToString()),
						InferredType = typeof(T),
						RequestedType = typeof(T),
						LocalValue = array[i]
					};

				values[i] = (T) context.RootSerializer.Deserialize(newContext);
			}
			return values;
		}
	}
}