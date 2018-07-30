using Manatee.Json.Path;
using Manatee.Json.Schema;
using Manatee.Json.Tests.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Path
{
	[TestFixture]
	public class ClientTests
	{
		[Test]
		public void Issue16_PathIntolerantOfUndscoresInPropertyNames()
		{
			var expected = JsonPathWith.Name("properties").Name("id_person");

			var actual = JsonPath.Parse("$.properties.id_person");

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		[Test]
		public void Issue17_UriValidationIntolerantOfLocalHost()
		{
			var text = "{\"id\": \"http://localhost/json-schemas/address.json\"}";
			var json = JsonValue.Parse(text);

			var results = JsonSchema04.MetaSchema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void Issue31_JsonPathArrayOperatorShouldWorkOnObjects_Parsed()
		{
			var json = GoessnerExamplesTest.GoessnerData;
			var path = JsonPath.Parse(@"$.store.bicycle[?(1==1)]");

			var results = path.Evaluate(json);

			Assert.AreEqual(new JsonArray { "red", 19.95 }, results);
		}

		[Test]
		public void Issue31_JsonPathArrayOperatorShouldWorkOnObjects_Constructed()
		{
			var json = GoessnerExamplesTest.GoessnerData;
			var path = JsonPathWith.Name("store")
			                       .Name("bicycle")
			                       .Array(jv => 1 == 1);

			var results = path.Evaluate(json);

			Assert.AreEqual(new JsonArray { "red", 19.95 }, results);
		}
	}
}
