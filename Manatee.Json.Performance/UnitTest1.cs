using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Performance
{
	[TestClass]
	[DeploymentItem("Associates.json")]
	public class Performace
	{
		// Time to beat: 00:00:00.0570000
		[TestMethod]
		public void Performance_Parse_Single()
		{
			var content = File.ReadAllText("Associates.json");
			var start = DateTime.Now;
			var json = JsonValue.Parse(content);
			var end = DateTime.Now;
			Console.WriteLine(json);
			Console.WriteLine(end - start);
		}
		// Time to beat: 00:00:04.1-ish
		// Time to beat: 00:00:02.5930000
		// Time to beat: 00:00:00.8660866
		[TestMethod]
		public void Performance_Parse_10000()
		{
			var content = File.ReadAllText("Associates.json");
			var start = DateTime.Now;
			JsonValue json = null;
			for (int i = 0; i < 10000; i++)
			{
				json = JsonValue.Parse(content);
			}
			var end = DateTime.Now;
			Console.WriteLine(json);
			Console.WriteLine(end - start);
		}
	}
}
