using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	public class SchemaTest : IJsonSerializable
	{
		public string Description { get; set; }
		public JsonValue Data { get; set; }
		public bool Valid { get; set; }
		public SchemaTestOutputSet Output { get; set; }
		public SchemaValidationResults OutputGeneration { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Description = obj.TryGetString("description");
			Data = obj["data"];
			Valid = obj["valid"].Boolean;
			var output = obj.TryGetObject("output");
			if (output != null)
				Output = serializer.Deserialize<SchemaTestOutputSet>(output);
			var results = obj.TryGetObject("outputGeneration")?.TryGetObject("verbose");
			if (results != null)
				OutputGeneration = serializer.Deserialize<SchemaValidationResults>(results);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var obj = new JsonObject
				{
					["description"] = Description,
					["data"] = Data,
					["valid"] = Valid
				};

			if (JsonSchemaOptions.Default.ConfigureForTestOutput && OutputGeneration != null)
				obj["output"] = new JsonObject
					{
						["basic"] = OutputGeneration.Flatten().ToJson(serializer),
						["detailed"] = OutputGeneration.Condense().ToJson(serializer),
						["verbose"] = OutputGeneration.ToJson(serializer)
					};
			else if (Output != null)
				obj["output"] = serializer.Serialize(Output);

			return obj;
		}
	}
}