using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ObjectSerializer : GenericTypeSerializerBase
	{
		public override bool Handles(Type type, JsonSerializerOptions options, JsonValue json)
		{
			return type == typeof(object);
		}

		private static JsonValue _Encode(object input, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		private static object _Decode(JsonValue json, JsonSerializer serializer)
		{
			switch (json.Type)
			{
				case JsonValueType.Number:
					return json.Number;
				case JsonValueType.String:
					return json.String;
				case JsonValueType.Boolean:
					return json.Boolean;
				case JsonValueType.Array:
					return json.Array.Select(serializer.Deserialize<object>).ToList();
				case JsonValueType.Object:
					var result = new ExpandoObject() as IDictionary<string, object>;
					foreach (var kvp in json.Object)
					{
						result[kvp.Key] = serializer.Deserialize<object>(kvp.Value);
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