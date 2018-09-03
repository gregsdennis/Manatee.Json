using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class AnyOfSchemaTest
	{
		public static IEnumerable ValidateReturnsErrorOnNoneValidData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema()
					                              .AnyOf(new JsonSchema().Type(JsonSchemaType.Array),
					                                     new JsonSchema().Type(JsonSchemaType.Number)));
			}
		}
		[TestCaseSource(nameof(ValidateReturnsErrorOnNoneValidData))]
		public void ValidateReturnsErrorOnNoneValid(JsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		public static IEnumerable ValidateReturnsValidOnSingleValidData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema()
					                              .AllOf(new JsonSchema()
						                                     .Type(JsonSchemaType.Number)
						                                     .Minimum(10),
					                                     new JsonSchema()
						                                     .Type(JsonSchemaType.Number)
						                                     .Maximum(20)));
			}
		}
		[TestCaseSource(nameof(ValidateReturnsValidOnSingleValidData))]
		public void ValidateReturnsValidOnSingleValid(JsonSchema schema)
		{
			var json = (JsonValue) 25;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}