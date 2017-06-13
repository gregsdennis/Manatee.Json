namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class StringSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			return obj as string;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			return (T) (object) json.String;
		}
	}
}