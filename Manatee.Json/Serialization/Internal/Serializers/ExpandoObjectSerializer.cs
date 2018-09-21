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
			return dict.ToDictionary(kvp => kvp.Key, kvp =>
					{
						var newContext = new SerializationContext
							{
								RootSerializer = context.RootSerializer,
								JsonRoot = context.JsonRoot,
								CurrentLocation = context.CurrentLocation.CloneAndAppend(kvp.Key),
								InferredType = kvp.Value?.GetType() ?? typeof(object),
								RequestedType = typeof(object),
								Source = kvp.Value
							};

						return context.RootSerializer.Serialize(newContext);
					}).ToJson();
		}
		private static ExpandoObject _Decode(SerializationContext context)
		{
			throw new NotImplementedException();
		}
	}
}