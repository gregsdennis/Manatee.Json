using System;
using System.Collections.Generic;
using System.Text;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class UnevaluatedItemsTests
	{
		[TestCase("[true,\"hello\", false]", true)]
		[TestCase("[1,3,4]", true)]
		[TestCase("[true, false]", false)]
		public void SpecIssue810_EdgeCases(string jsonText, bool passes)
		{
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Array)
				.OneOf(new JsonSchema().Contains(new JsonSchema().Type(JsonSchemaType.String)),
					   new JsonSchema().Items(new JsonSchema().Type(JsonSchemaType.Integer)))
				.UnevaluatedItems(new JsonSchema().Type(JsonSchemaType.Boolean));
			var json = JsonValue.Parse(jsonText);

			var result = schema.Validate(json);

			if (passes)
				result.AssertValid();
			else
				result.AssertInvalid();
		}
	}
}
