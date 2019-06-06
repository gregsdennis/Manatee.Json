using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	public class SchemaTest : IJsonSerializable
	{
		public string Description { get; set; }
		public JsonValue Data { get; set; }
		public bool Valid { get; set; }
		public SchemaValidationResults Results { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Description = obj.TryGetString("description");
			Data = obj["data"];
			Valid = obj["valid"].Boolean;
			var results = obj.TryGetObject("output")?.TryGetObject("verbose");
			if (results != null)
				Results = serializer.Deserialize<SchemaValidationResults>(results);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject
				{
					["description"] = Description,
					["data"] = Data,
					["valid"] = Valid,
					["output"] = new JsonObject
						{
							["basic"] = Results.Flatten().ToJson(serializer),
							["detailed"] = Results.Condense().ToJson(serializer),
							["verbose"] = Results.ToJson(serializer)
						}
				};
		}
	}
}