using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class BooleanSchemaTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema().Type(JsonSchemaType.Boolean));
			}
		} 
		
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsErrorOnNonBoolean(JsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}

		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsValidOnBoolean(JsonSchema schema)
		{
			var json = (JsonValue) false;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}
