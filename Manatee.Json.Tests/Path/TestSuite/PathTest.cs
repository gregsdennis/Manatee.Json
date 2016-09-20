using System;
using Manatee.Json.Path;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Path.TestSuite
{
	internal class PathTest : IJsonSerializable
	{
		public JsonPath Path { get; private set; }
		public JsonValue Result { get; private set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Path = JsonPath.Parse(json.Object["path"].String);
			Result = json.Object["result"];
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}