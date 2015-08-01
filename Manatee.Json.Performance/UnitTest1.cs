using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Performance
{
	[TestClass]
	[DeploymentItem("Associates.json")]
	public class Performace
	{
		[TestMethod]
		public void Performance_Deserialize_Single()
		{
			var serializer = new JsonSerializer { Options = { CaseSensitiveDeserialization = false } };
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			var content = File.ReadAllText("Associates.json");
			var json = JsonValue.Parse(content);
			serializer.Deserialize<IEnumerable<Associate>>(json);
		}
		[TestMethod]
		public void Performance_Deserialize_10000()
		{
			var serializer = new JsonSerializer { Options = { CaseSensitiveDeserialization = false } };
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			var content = File.ReadAllText("Associates.json");
			var json = JsonValue.Parse(content);
			for (int i = 0; i < 10000; i++)
			{
				serializer.Deserialize<IEnumerable<Associate>>(json);
			}
		}
	}
}
