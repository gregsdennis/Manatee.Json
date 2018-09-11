using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class AllOfSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnAnyInvalid()
		{
			var schema = new JsonSchema()
				.AllOf(new JsonSchema().Type(JsonSchemaType.Array),
				       new JsonSchema().Type(JsonSchemaType.Number));

			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		[Test]
		public void ValidateReturnsValidOnAllValid()
		{
			var schema = new JsonSchema()
				.AllOf(new JsonSchema()
					       .Type(JsonSchemaType.Number)
					       .Minimum(10),
				       new JsonSchema()
					       .Type(JsonSchemaType.Number)
					       .Maximum(20));

			var json = (JsonValue) 15;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}