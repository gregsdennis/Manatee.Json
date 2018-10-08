using System;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class EnumValueSerializer : IPrioritizedSerializer
	{
		public int Priority => 2;

		public bool ShouldMaintainReferences => false;

		public bool Handles(SerializationContext context)
		{
			var serializing = context.LocalValue == null;
			return context.InferredType.GetTypeInfo().IsEnum &&
			       ((serializing && context.RootSerializer.Options.EnumSerializationFormat == EnumSerializationFormat.AsInteger) || // used during serialization
			        (!serializing && context.LocalValue?.Type == JsonValueType.Number)); // used during deserialization
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var value = Convert.ToInt32(context.Source);
			return value;
		}
		public object Deserialize(SerializationContext context)
		{
			var value = (int)context.LocalValue.Number;
			return Enum.ToObject(context.InferredType, value);
		}
	}
}