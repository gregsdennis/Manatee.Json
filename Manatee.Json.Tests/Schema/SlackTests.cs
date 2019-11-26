using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	public class SlackTests
	{
		[Test]
		public void Test()
		{
			var schema = new JsonSchema()
				.UnevaluatedProperties(false)
				.AllOf(new JsonSchema()
					       .Property("foo", new JsonSchema().Type(JsonSchemaType.String | JsonSchemaType.Null))
					       .Property("bar", new JsonSchema().Type(JsonSchemaType.String | JsonSchemaType.Null)),
				       new JsonSchema()
					       .AdditionalProperties(new JsonSchema().Not(new JsonSchema().Enum(JsonValue.Null))));
			var json = new JsonObject
				{
					["bar"] = "foo",
					["bob"] = "who?"
				};

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}
