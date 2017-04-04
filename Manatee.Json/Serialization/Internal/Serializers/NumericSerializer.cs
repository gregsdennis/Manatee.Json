using System;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class NumericSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			double value = Convert.ToDouble(obj);
			return value;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var value = json.Number;
			return (T) Convert.ChangeType(value, typeof (T));
		}
	}
}