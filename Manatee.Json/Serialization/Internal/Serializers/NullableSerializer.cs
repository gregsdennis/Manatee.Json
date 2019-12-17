using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	[UsedImplicitly]
	internal class NullableSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContextBase context)
		{
			return context.InferredType.GetTypeInfo().IsGenericType &&
			       context.InferredType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		[UsedImplicitly]
		private static JsonValue _Encode<T>(SerializationContext context)
			where T : struct
		{
			var nullable = (T?) context.Source;
			if (!nullable.HasValue) return JsonValue.Null;

			var encodeDefaultValues = context.RootSerializer.Options.EncodeDefaultValues;
			T @default = default;
			context.RootSerializer.Options.EncodeDefaultValues = Equals(nullable.Value, @default);
			context.Push(typeof(T), typeof(T), null!, nullable.Value);
			var json = context.RootSerializer.Serialize(context);
			context.Pop();
			context.RootSerializer.Options.EncodeDefaultValues = encodeDefaultValues;

			return json;
		}
		[UsedImplicitly]
		private static T? _Decode<T>(DeserializationContext context)
			where T : struct
		{
			if (context.LocalValue == JsonValue.Null) return null;

			// this is a special case for the pointer in that we don't want to add a segment,
			// but we have to push something so that it can be popped.
			context.Push(typeof(T), null!, context.LocalValue);
			var value = (T) context.RootSerializer.Deserialize(context)!;
			context.Pop();

			return value;
		}
	}
}