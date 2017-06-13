namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class RegisteredObjectSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => true;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			JsonValue json;
			serializer.Encode(obj, out json);
			return json;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			T value;
			serializer.Decode(json, out value);
			return value;
		}
	}
}