using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class BooleanSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNonBoolean()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Boolean};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnBoolean()
		{
			var schema = new JsonSchema04 { Type = JsonSchemaTypeDefinition.Boolean };
			var json = (JsonValue) false;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
