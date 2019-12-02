using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ObjectSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(SerializationContextBase context)
		{
			return context.InferredType == typeof(object);
		}

		private static JsonValue _Encode(SerializationContext context)
		{
			throw new NotImplementedException();
		}
		private static object _Decode(DeserializationContext context)
		{
			switch (context.LocalValue.Type)
			{
				case JsonValueType.Number:
					return context.LocalValue.Number;
				case JsonValueType.String:
					return context.LocalValue.String;
				case JsonValueType.Boolean:
					return context.LocalValue.Boolean;
				case JsonValueType.Array:
					return context.LocalValue.Array.Select((value, i) =>
						{
							context.Push(typeof(object), typeof(object), i.ToString(), value);
							var json = context.RootSerializer.Deserialize(context);
							context.Pop();
							return json;
						}).ToList();
				case JsonValueType.Object:
					var result = new ExpandoObject() as IDictionary<string, object>;
					foreach (var kvp in context.LocalValue.Object)
					{
						var newContext = new DeserializationContext(context)
						{
								CurrentLocation = context.CurrentLocation.CloneAndAppend(kvp.Key),
								InferredType = typeof(object),
								RequestedType = typeof(object),
								LocalValue = kvp.Value
							};
						result[kvp.Key] = context.RootSerializer.Deserialize(newContext);
					}
					return result;
				case JsonValueType.Null:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}