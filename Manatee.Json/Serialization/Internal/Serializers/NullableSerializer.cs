using System;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class NullableSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType &&
			       context.InferredType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		private static JsonValue _Encode<T>(SerializationContext<T?> context)
			where T : struct
		{
			if (!context.Source.HasValue) return JsonValue.Null;
			var encodeDefaultValues = context.RootSerializer.Options.EncodeDefaultValues;
			context.RootSerializer.Options.EncodeDefaultValues = Equals(context.Source.Value, default (T));
			var json = context.RootSerializer.Serialize(context.Source.Value);
			context.RootSerializer.Options.EncodeDefaultValues = encodeDefaultValues;
			return json;
		}
		private static T? _Decode<T>(SerializationContext<JsonValue> context)
			where T : struct
		{
			if (context.Source == JsonValue.Null)
				return null;
			T? nullable = context.RootSerializer.Deserialize<T>(context.Source);
			return nullable;
		}
	}
}