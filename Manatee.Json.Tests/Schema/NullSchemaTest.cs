using System.Collections;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class NullSchemaTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new JsonSchema().Type(JsonSchemaType.Null);
			}
		}
		
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsErrorOnNonNull(JsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			results.AssertInvalid();
		}
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsValidOnNull(JsonSchema schema)
		{
			var json = JsonValue.Null;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}
