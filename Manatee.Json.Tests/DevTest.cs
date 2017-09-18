using System;
using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Schema;
using Manatee.Json.Tests.Schema.TestSuite;
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
			var s = "#";
			
			Assert.IsTrue(Uri3986.IsValid(s));
		}
	}
}