using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ConstSchemaTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			JsonOptions.LogCategory = LogCategory.Schema;
		}

		[Test]
		public void ValidationFails()
		{
			var schema = new JsonSchema().Const(5);

			JsonValue json = 6;

			var results = schema.Validate(json);

			Assert.IsFalse(results.IsValid);
		}

		[Test]
		public void ValidationPasses()
		{
			var schema = new JsonSchema().Const(5);

			JsonValue json = 5;

			var results = schema.Validate(json);

			Assert.IsTrue(results.IsValid);
		}
	}
}
