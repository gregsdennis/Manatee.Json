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
		public void Draft04_ConcoctedExampleFails()
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
			var json = new JsonObject {{"type", "number"}};

			var results = schema.Validate(json);

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
		[Test]
		public void Draft06_ConcoctedExampleFails()
		{
			// This test is intended to demontrate that it's not possible to create
			// a primitive definition; you must use the built-in definitions.
			// The type definition equality logic relies on this fact.
			var schema = new JsonSchema06
				{
					Type = new JsonSchemaTypeDefinition("number")
						{
							Definition = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Number
								}
						}
				};
			var json = new JsonObject {{"type", "number"}};

			var results = schema.Validate(json);

			Assert.IsFalse(results.Valid);
		}
	}
}
