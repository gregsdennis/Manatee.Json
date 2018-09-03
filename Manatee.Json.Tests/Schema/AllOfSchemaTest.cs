using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class AllOfSchemaTest
	{
		public static IEnumerable ValidateReturnsErrorOnAnyInvalidData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema()
					                              .AllOf(new JsonSchema().Type(JsonSchemaType.Array),
					                                     new JsonSchema().Type(JsonSchemaType.Number)));
			}
		}
		[TestCaseSource(nameof(ValidateReturnsErrorOnAnyInvalidData))]
		public void ValidateReturnsErrorOnAnyInvalid(JsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		public static IEnumerable ValidateReturnsValidOnAllValidData
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
		[TestCaseSource(nameof(ValidateReturnsValidOnAllValidData))]
		public void ValidateReturnsValidOnAllValid(JsonSchema schema)
		{
			var json = (JsonValue) 15;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}