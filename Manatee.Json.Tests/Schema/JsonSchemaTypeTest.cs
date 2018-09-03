using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class JsonSchemaTypeTest
	{
		public static IEnumerable TestCases
		{
			get
			{
				yield return new TestCaseData(MetaSchemas.Draft04);
				yield return new TestCaseData(MetaSchemas.Draft06);
				yield return new TestCaseData(MetaSchemas.Draft07);
				yield return new TestCaseData(MetaSchemas.Draft08);
			}
		}

		[TestCaseSource(nameof(TestCases))]
		public void Draft04_PrimitiveSchemaSucceeds(JsonSchema schema)
		{
			var json = new JsonObject {{"type", "integer"}};

			var results = schema.Validate(json);

			Assert.IsTrue(results.IsValid);
		}
		[TestCaseSource(nameof(TestCases))]
		public void Draft04_NonPrimitiveStringSchemaFails(JsonSchema schema)
		{
			var json = new JsonObject {{"type", "other"}};

			var results = schema.Validate(json);

			Assert.IsFalse(results.IsValid);
		}
	}
}
