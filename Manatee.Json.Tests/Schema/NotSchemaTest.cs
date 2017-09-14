using System.Collections;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class NotSchemaTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number, Maximum = 10};
				yield return new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number, Maximum = 10};
			}
		} 
		
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsErrorOnInvalid(IJsonSchema schema)
		{
			var json = new JsonArray();

			var results = schema.Validate(json);

			Assert.AreEqual(1, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsValid(IJsonSchema schema)
		{
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}