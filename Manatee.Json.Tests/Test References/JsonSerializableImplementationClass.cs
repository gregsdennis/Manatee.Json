using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Test_References
{
	public class JsonSerializableImplementationClass : ImplementationClass, IJsonSerializable
	{
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			RequiredProp = json.Object["requiredProp"].String;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject { { "requiredProp", RequiredProp } };
		}
	}
}