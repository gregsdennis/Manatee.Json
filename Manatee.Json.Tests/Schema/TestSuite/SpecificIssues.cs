using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	public class SpecificIssues
	{
		[Test]
		[TestCase("[true,false]", true)]
		[TestCase("[\"yes\",\"no\"]", true)]
		[TestCase("[\"yes\",false]", false)]
		public void Issue291(string instanceText, bool shouldPass)
		{
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Verbose;
			var schema = new JsonSchema()
				.UnevaluatedItems(new JsonSchema().Type(JsonSchemaType.Boolean))
				.AnyOf(new JsonSchema()
					       .Items(new JsonSchema().Type(JsonSchemaType.String)),
				       true);
			var instance = JsonValue.Parse(instanceText);

			var results = schema.Validate(instance);

			if (shouldPass)
				results.AssertValid();
			else
				results.AssertInvalid();
		}
	}
}
