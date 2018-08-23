using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	// TODO: Add categories to exclude this test.
	//[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		public void Test()
		{
			JsonValue json = new JsonObject
				{
					["type"] = "object",
					["customKeyword"] = new JsonArray {1, 2, 3}
				};
			var schema = new JsonSchema
				{
					Keywords =
						{
							new TypeKeyword {Type = JsonSchemaType.Object},
							new OtherKeyword {Name = "customKeyword", Content = new JsonArray {1, 2, 3}}
						}
				};

			Assert.IsTrue(schema.Validate(new JsonObject()).IsValid);

			Assert.IsFalse(schema.Validate("this").IsValid);

			Assert.AreEqual(json, schema.ToJson(new JsonSerializer()));
		}
	}
}