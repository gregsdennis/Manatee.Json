using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Performance
{
	[TestClass]
	[DeploymentItem("Associates.json")]
	public class ParseTest
	{
		[TestMethod]
		public void Performance_Parse_Single()
		{
			Console.WriteLine("Time To Beat: 00:00:00.0110006");
			var content = File.ReadAllText("Associates.json");
			var start = DateTime.Now;
			var json = JsonValue.Parse(content);
			var end = DateTime.Now;
			Console.WriteLine(json);
			Console.WriteLine(end - start);
		}
		[TestMethod]
		public void Performance_Parse_10000()
		{
			Console.WriteLine("Time To Beat: 00:00:00.3410196");
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