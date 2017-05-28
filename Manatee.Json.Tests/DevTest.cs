using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	//[Ignore]
	public class DevTest
	{
		[TestMethod]
		public void Test1()
		{
			var text = "http://www.google.com/file/";
			var uri = new Uri(text);

			Console.WriteLine(uri);
		}
	}
}