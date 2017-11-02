using System;
using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Schema.TestSuite;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		// TODO: Add categories to exclude this test.
		public void Test1()
		{
			var list = new List<object> {1, false, "hello"};

			var serializer = new JsonSerializer();
			var json = serializer.Serialize(list);

			Console.WriteLine(json);
		}
	}
}