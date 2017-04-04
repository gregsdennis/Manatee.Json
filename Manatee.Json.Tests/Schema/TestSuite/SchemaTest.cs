using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	public class SchemaTest : IJsonSerializable
	{
		public string Description { get; set; }
		public JsonValue Data { get; set; }
		public bool Valid { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Description = obj.TryGetString("description");
			Data = obj["data"];
			Valid = obj["valid"].Boolean;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}