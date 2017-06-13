using System;
using System.Collections.Generic;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	public class SchemaTestSet : IJsonSerializable
	{
		public string Description { get; set; }
		public IJsonSchema Schema { get; set; }
		public IEnumerable<SchemaTest> Tests { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Description = obj["description"].String;
			Schema = serializer.Deserialize<IJsonSchema>(obj["schema"]);
			Tests = serializer.Deserialize<List<SchemaTest>>(obj["tests"]);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}