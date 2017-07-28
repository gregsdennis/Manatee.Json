using System;
using System.Collections.Generic;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class DevTest
	{
		[TestMethod]
		[Ignore]
		public void Test1()
		{
			var text = "http://www.google.com/file/";
			var uri = new Uri(text);

			Console.WriteLine(uri);
		}
	}
}