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
			Assert.AreEqual(JsonSchemaType.Integer, JsonSchemaType.Integer);
		}
		[Test]
		public void DifferentStandardDefinitionsNotEqual()
		{
			Assert.AreNotEqual(JsonSchemaType.Integer, JsonSchemaType.String);
		}
	}
}
