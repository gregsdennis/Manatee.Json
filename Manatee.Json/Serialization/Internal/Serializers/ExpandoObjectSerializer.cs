using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ExpandoObjectSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContext context)
		{
			return context.InferredType == typeof(ExpandoObject);
		}

		private static JsonValue _Encode(SerializationContext context)
		{
			var dict = (IDictionary<string, object>)context.Source;
			return dict.ToDictionary(kvp => kvp.Key, kvp => context.RootSerializer.Serialize<dynamic>(kvp.Value))
			           .ToJson();
		}
		private static ExpandoObject _Decode(SerializationContext context)
		{
			throw new NotImplementedException();
		}
	}
}