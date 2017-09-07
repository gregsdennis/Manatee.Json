using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class NullSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNonNull()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Null};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[Test]
		public void ValidateReturnsValidOnNull()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Null};
			var json = JsonValue.Null;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
