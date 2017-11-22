namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class RegisteredObjectSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => true;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			serializer.CustomSerializations.Encode(serializer, obj, out var json);
			return json;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			serializer.CustomSerializations.Decode(serializer, json, out T value);
			return value;
		}
	}
}