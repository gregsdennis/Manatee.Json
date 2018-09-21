using System;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class EnumValueSerializer : IPrioritizedSerializer
	{
		public int Priority => -10;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			var json = (context as SerializationContext<JsonValue>)?.Source;

			return context.InferredType.GetTypeInfo().IsEnum &&
			       (context.RootSerializer.Options.EnumSerializationFormat == EnumSerializationFormat.AsInteger || // used during serialization
			        json?.Type == JsonValueType.Number); // used during deserialization
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			var value = Convert.ToInt32(context.Source);
			return value;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			var value = (int)context.Source.Number;
			return (T) Enum.ToObject(typeof (T), value);
		}
	}
}