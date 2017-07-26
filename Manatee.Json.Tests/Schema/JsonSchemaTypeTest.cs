using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class JsonSchemaTypeTest
	{
		[TestMethod]
		public void PrimitiveSchemaSucceeds()
		{
			var json = new JsonObject {{"type", "integer"}};

			var results = JsonSchema04.MetaSchema.Validate(json);

			Assert.IsTrue(results.Valid);
		}
		[TestMethod]
		public void NonPrimitiveStringSchemaFails()
		{
			var json = new JsonObject {{"type", "other"}};

			var results = JsonSchema04.MetaSchema.Validate(json);

			Assert.IsFalse(results.Valid);
		}
		[TestMethod]
		public void ConcoctedExampleFails()
		{
			// This test is intended to demontrate that it's not possible to create
			// a primitive definition; you must use the built-in definitions.
			// The type definition equality logic relies on this fact.
			var schema = new JsonSchema04
				{
					Type = new JsonSchemaTypeDefinition("number")
						{
							Definition = new JsonSchema04
								{
									Type = JsonSchemaTypeDefinition.Number
								}
						}
				};
			var json = new JsonObject { { "type", "number" } };

			var results = schema.Validate(json);

			Assert.IsFalse(results.Valid);
		}
	}
}
