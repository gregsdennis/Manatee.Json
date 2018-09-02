using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ExamplesSchemaTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new TestCaseData(JsonSchema06.MetaSchema);
				yield return new TestCaseData(JsonSchema07.MetaSchema);
			}
		}

		[TestCaseSource(nameof(TestData))]
		public void ExamplesNotAnArray(JsonSchema metaSchema)
		{
			var json = new JsonObject
				{
					["examples"] = 5
				};

			var results = metaSchema.Validate(json);
			
			Assert.IsFalse(results.IsValid);
		}
	}
}
