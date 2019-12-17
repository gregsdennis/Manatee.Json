using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using JetBrains.Annotations;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	[UsedImplicitly]
	internal class ExpandoObjectSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContextBase context)
		{
			return context.InferredType == typeof(ExpandoObject);
		}

		[UsedImplicitly]
		private static JsonValue _Encode(SerializationContext context)
		{
			var dict = (IDictionary<string, object?>)context.Source!;
			return dict.ToDictionary(kvp => kvp.Key, kvp =>
					{
						context.Push(kvp.Value?.GetType() ?? typeof(object), typeof(object), kvp.Key, kvp.Value);
						var value = context.RootSerializer.Serialize(context);
						context.Pop();

						return value;
					})!.ToJson();
		}
		[UsedImplicitly]
		private static ExpandoObject _Decode(DeserializationContext context)
		{
			throw new NotImplementedException();
		}
	}
}