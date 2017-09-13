using System;
using System.Collections.Generic;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class DevTest
	{
		[Test]
		// TOOD: Add categories to exclude this test.
		[Ignore("This test for development purposes only.")]
		public void Test1()
		{
			var text = "http://www.google.com/file/";
			var uri = new Uri(text);

			Console.WriteLine(uri);
		}
	}
}