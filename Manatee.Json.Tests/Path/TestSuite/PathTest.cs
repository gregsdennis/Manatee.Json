using System;
using Manatee.Json.Path;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Path.TestSuite
{
	internal class PathTest : IJsonSerializable
	{
		public JsonPath Path { get; private set; }
		public JsonArray Result { get; private set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Path = JsonPath.Parse(json.Object["path"].String);
			var result = json.Object["result"];
			if (result.Type == JsonValueType.Boolean && !result.Boolean)
				Result = new JsonArray();
			else
				Result = result.Array;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}