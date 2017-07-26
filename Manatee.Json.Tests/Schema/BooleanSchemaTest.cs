using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class BooleanSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNonBoolean()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Boolean};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
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
