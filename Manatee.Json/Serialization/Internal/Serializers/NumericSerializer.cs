using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class NumericSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public bool Handles(Type type, JsonSerializerOptions options)
		{
			return type.IsNumber();
		}
		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var value = Convert.ToDouble(obj);
			return value;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = json.Number;
			return (T) Convert.ChangeType(value, typeof (T));
		}
	}
}