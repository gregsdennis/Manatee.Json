using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ExamplesSchemaTest
	{
		[Test]
		public void ExamplesNotAnArray()
		{
			var json = new JsonObject
				{
					["examples"] = 5
				};

			var results = JsonSchema06.MetaSchema.Validate(json);
			
			Assert.IsFalse(results.Valid);
		}
	}
}
