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
				yield return new TestCaseData(MetaSchemas.Draft04) {TestName = nameof(MetaSchemas.Draft04)};
				yield return new TestCaseData(MetaSchemas.Draft06) {TestName = nameof(MetaSchemas.Draft06)};
				yield return new TestCaseData(MetaSchemas.Draft07) {TestName = nameof(MetaSchemas.Draft07)};
				//yield return new TestCaseData(MetaSchemas.Draft08) {TestName = nameof(MetaSchemas.Draft08)};
			}
		}

		[TestCaseSource(nameof(TestCases))]
		public void PrimitiveSchemaSucceeds(JsonSchema metaSchema)
		{
			var customSchema = new JsonObject {{"type", "integer"}};

			var results = metaSchema.Validate(customSchema);

			Assert.IsTrue(results.IsValid);
		}
		[TestCaseSource(nameof(TestCases))]
		public void NonPrimitiveStringSchemaFails(JsonSchema metaSchema)
		{
			var customSchema = new JsonObject {{"type", "other"}};

			var results = metaSchema.Validate(customSchema);

			Assert.IsFalse(results.IsValid);
		}
	}
}
