using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	public class SchemaTestOutputSet : IJsonSerializable
	{
		public SchemaValidationResults Basic { get; set; }
		public SchemaValidationResults Detailed { get; set; }
		public SchemaValidationResults Verbose { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Basic = serializer.Deserialize<SchemaValidationResults>(obj["basic"]);
			Detailed = serializer.Deserialize<SchemaValidationResults>(obj["detailed"]);
			Verbose = serializer.Deserialize<SchemaValidationResults>(obj["verbose"]);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject
				{
					["basic"] = Basic.ToJson(serializer),
					["detailed"] = Detailed.ToJson(serializer),
					["verbose"] = Verbose.ToJson(serializer)
				};
		}
	}
}