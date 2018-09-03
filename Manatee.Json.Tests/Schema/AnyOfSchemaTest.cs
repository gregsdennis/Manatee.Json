using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class AnyOfSchemaTest
	{
		[Test]
		public void ValidateReturnsErrorOnNoneValid()
		{
			var schema = new JsonSchema()
				.AnyOf(new JsonSchema().Type(JsonSchemaType.Array),
				       new JsonSchema().Type(JsonSchemaType.Number));

			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		[Test]
		public void ValidateReturnsValidOnSingleValid()
		{
			var schema = new JsonSchema()
				.AnyOf(new JsonSchema()
					       .Type(JsonSchemaType.Number)
					       .Minimum(10),
				       new JsonSchema()
					       .Type(JsonSchemaType.Number)
					       .Maximum(20));


			var json = (JsonValue) 25;

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void ValidateReturnsValidOnMultipleValid()
		{
			var schema = new JsonSchema()
				.AnyOf(new JsonSchema()
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