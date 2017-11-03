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
			dynamic dyn = new ExpandoObject();
			dyn.StringProp = "string";
			dyn.IntProp = 5;
			dyn.NestProp = new ExpandoObject();
			dyn.NestProp.Value = new ObjectWithBasicProps
				{
					BoolProp = true
				};

			JsonValue expected = new JsonObject
				{
					["StringProp"] = "string",
					["IntProp"] = 5,
					["NestProp"] = new JsonObject
						{
							["Value"] = new JsonObject
								{
									["BoolProp"] = true
								}
						}
				};

			var serializer = new JsonSerializer
				{
					Options =
						{
							TypeNameSerializationBehavior = TypeNameSerializationBehavior.OnlyForAbstractions
						}
				};
			var json = serializer.Serialize<dynamic>(dyn);

			Assert.AreEqual(expected, json);
		}
	}
}