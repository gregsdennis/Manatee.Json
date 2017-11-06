using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class JsonSchemaTypeTest
	{
		[Test]
		public void Draft04_PrimitiveSchemaSucceeds()
		{
			var json = new JsonObject {{"type", "integer"}};

			var results = JsonSchema04.MetaSchema.Validate(json);

			Assert.IsTrue(results.Valid);
		}
		[Test]
		public void Draft04_NonPrimitiveStringSchemaFails()
		{
			var json = new JsonObject {{"type", "other"}};

			var results = JsonSchema04.MetaSchema.Validate(json);

			Assert.IsFalse(results.Valid);
		}
		[Test]
		public void Draft06_PrimitiveSchemaSucceeds()
		{
			var json = new JsonObject {{"type", "integer"}};

			var results = JsonSchema06.MetaSchema.Validate(json);

			Assert.IsTrue(results.Valid);
		}
		[Test]
		public void Draft06_NonPrimitiveStringSchemaFails()
		{
			var json = new JsonObject {{"type", "other"}};

			var results = JsonSchema06.MetaSchema.Validate(json);

			Assert.IsFalse(results.Valid);
		}
	}
}
