using System;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class NullableSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContextBase context)
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
			context.Push(typeof(T), typeof(T), null, nullable.Value);
			var json = context.RootSerializer.Serialize(context);
			context.Pop();
			context.RootSerializer.Options.EncodeDefaultValues = encodeDefaultValues;

			return json;
		}
		private static T? _Decode<T>(DeserializationContext context)
			where T : struct
		{
			if (context.LocalValue == JsonValue.Null) return null;

			var newContext = new DeserializationContext(context)
			{
					CurrentLocation = context.CurrentLocation.Clone(),
					InferredType = typeof(T),
					RequestedType = typeof(T),
					LocalValue = context.LocalValue
				};

			return (T) context.RootSerializer.Deserialize(newContext);
		}
	}
}