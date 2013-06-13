using Manatee.Json;
using Manatee.Json.Serialization;

namespace Manatee.Tests.Test_References
{
	public class JsonCompatibleImplementationClass : ImplementationClass, IJsonCompatible
	{
		public void FromJson(JsonValue json)
		{
			RequiredProp = json.Object["requiredProp"].String;
		}
		public JsonValue ToJson()
		{
			return new JsonObject {{"requiredProp", RequiredProp}};
		}
	}
}