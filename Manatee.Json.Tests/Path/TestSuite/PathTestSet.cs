using System;
using System.Collections.Generic;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Path.TestSuite
{
	internal class PathTestSet : IJsonSerializable
	{
		public string Title { get; private set; }
		public JsonValue Data { get; private set; }
		public IEnumerable<PathTest> Tests { get; private set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Title = json.Object["title"].String;
			Data = json.Object["data"];
			Tests = serializer.Deserialize<IEnumerable<PathTest>>(json.Object["tests"]);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
