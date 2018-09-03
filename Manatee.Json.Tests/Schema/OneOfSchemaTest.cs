using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class OneOfSchemaTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new JsonSchema()
					.OneOf(new JsonSchema().Type(JsonSchemaType.Number).Minimum(5),
					       new JsonSchema().Type(JsonSchemaType.Number).Minimum(10));
			}
		}
		
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsErrorOnNoneValid(JsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsErrorOnMoreThanOneValid(JsonSchema schema)
		{
			var json = (JsonValue) 20;

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsValidOnSingleValid(JsonSchema schema)
		{
			var json = (JsonValue) 7;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}