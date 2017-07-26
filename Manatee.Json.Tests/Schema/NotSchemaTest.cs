using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class NotSchemaTest
	{
		[TestMethod]
		public void ValidateReturnsErrorOnInvalid()
		{
			var schema = new JsonSchema04
				{
					Not = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array}
				};
			var json = new JsonArray();

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestMethod]
		public void ValidateReturnsValid()
		{
			var schema = new JsonSchema04
			{
					Not = new JsonSchema04 { Type = JsonSchemaTypeDefinition.Number,Maximum = 10}
				};
			var json = (JsonValue) 15;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}