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
	[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		public void Test()
		{
			JsonValue json = new JsonObject
				{
					["id"] = "http://json-schema.org/draft-04/schema#",
					["schema"] = "http://json-schema.org/draft-04/schema#",
					["type"] = "array",
					["const"] = new JsonArray {1, 2, 3}
				};
			var schema = new JsonSchema
				{
					new IdKeywordDraft04("http://json-schema.org/draft-04/schema#"),
					new SchemaKeywordDraft04(MetaSchemas.Draft04.Schema),
					new TypeKeyword(JsonSchemaType.Array),
					new ConstKeyword(new JsonArray {1, 2, 3})
				};

			var results = schema.ValidateSchema();
			Assert.IsTrue(results.IsValid, string.Join(Environment.NewLine, results.Errors.Select(e => e.Message)));

			results = schema.Validate(new JsonArray {1, 2, 3});
			Assert.IsTrue(results.IsValid, results.Errors.FirstOrDefault()?.Message);

			Assert.AreEqual(json, schema.ToJson(new JsonSerializer()));
		}
	}
}