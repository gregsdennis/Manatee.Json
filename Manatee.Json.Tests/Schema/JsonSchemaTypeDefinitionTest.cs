using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class JsonSchemaTypeDefinitionTest
	{
		[TestMethod]
		public void StandardDefinitionEqualsItself()
		{
			Assert.AreEqual(JsonSchemaTypeDefinition.Integer, JsonSchemaTypeDefinition.Integer);
		}
		[TestMethod]
		public void DifferentStandardDefinitionsNotEqual()
		{
			Assert.AreNotEqual(JsonSchemaTypeDefinition.Integer, JsonSchemaTypeDefinition.String);
		}
		[TestMethod]
		public void CannotRecreatePrimitiveDefinitions()
		{
			var constructed = new JsonSchema {Type = new JsonSchemaTypeDefinition("integer")};
			var predefined = new JsonSchema {Type = JsonSchemaTypeDefinition.Integer};

			Assert.AreNotEqual(predefined, constructed);
		}
	}
}
