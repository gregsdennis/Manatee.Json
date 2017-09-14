using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class JsonSchemaTypeDefinitionTest
	{
		[Test]
		public void StandardDefinitionEqualsItself()
		{
			Assert.AreEqual(JsonSchemaTypeDefinition.Integer, JsonSchemaTypeDefinition.Integer);
		}
		[Test]
		public void DifferentStandardDefinitionsNotEqual()
		{
			Assert.AreNotEqual(JsonSchemaTypeDefinition.Integer, JsonSchemaTypeDefinition.String);
		}
		[Test]
		public void CannotRecreatePrimitiveDefinitions()
		{
			var constructed = new JsonSchema04 {Type = new JsonSchemaTypeDefinition("integer")};
			var predefined = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer};

			Assert.AreNotEqual(predefined, constructed);
		}
	}
}
