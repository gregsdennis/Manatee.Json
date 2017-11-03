using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Schema.TestSuite;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	// TODO: Add categories to exclude this test.
	[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		public void Test1()
		{
			var list = new List<object> {1, false, "string", new ObjectWithBasicProps {DoubleProp = 5.5}};

			JsonValue expected = new JsonArray {1, false, "string", new JsonObject {["DoubleProp"] = 5.5}};

			var serializer = new JsonSerializer
				{
					Options =
						{
							TypeNameSerializationBehavior = TypeNameSerializationBehavior.OnlyForAbstractions
						}
				};
			var json = serializer.Serialize<dynamic>(list);

			Assert.AreEqual(expected, json);
		}
	}
}