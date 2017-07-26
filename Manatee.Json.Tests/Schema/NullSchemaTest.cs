using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class NullSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnNonNull()
		{
			var schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Null};
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
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
