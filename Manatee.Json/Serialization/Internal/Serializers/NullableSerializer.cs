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

		private static JsonValue _Encode<T>(SerializationContext context)
			where T : struct
		{
			var nullable = (T?) context.Source;
			if (!nullable.HasValue) return JsonValue.Null;

			var encodeDefaultValues = context.RootSerializer.Options.EncodeDefaultValues;
			context.RootSerializer.Options.EncodeDefaultValues = Equals(nullable.Value, default (T));
			var newContext = new SerializationContext
				{
					CurrentLocation = context.CurrentLocation.Clone(),
					InferredType = typeof(T),
					RequestedType = typeof(T),
					JsonRoot = context.JsonRoot,
					LocalValue = context.LocalValue,
					RootSerializer = context.RootSerializer
				};
			var json = context.RootSerializer.Serialize(newContext);
			context.RootSerializer.Options.EncodeDefaultValues = encodeDefaultValues;

			return json;
		}
		private static T? _Decode<T>(SerializationContext context)
			where T : struct
		{
			if (context.LocalValue == JsonValue.Null) return null;

			var newContext = new SerializationContext
				{
					CurrentLocation = context.CurrentLocation.Clone(),
					InferredType = typeof(T),
					RequestedType = typeof(T),
					JsonRoot = context.JsonRoot,
					LocalValue = context.LocalValue,
					RootSerializer = context.RootSerializer
				};

			return (T) context.RootSerializer.Deserialize(newContext);
		}
	}
}