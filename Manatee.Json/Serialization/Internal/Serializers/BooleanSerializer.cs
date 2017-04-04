namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class BooleanSerializer : ISerializer
	{
		public bool ShouldMaintainReferences => false;

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			return (bool) (object) obj;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			return (T) (object) json.Boolean;
		}
	}
}