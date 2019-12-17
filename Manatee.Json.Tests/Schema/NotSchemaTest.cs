using System.Collections;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class NotSchemaTest
	{
		[OneTimeSetUp]
		public void Setup()
		{
			JsonOptions.LogCategory = LogCategory.Schema;
		}

		public static IEnumerable TestData
		{
			get
			{
				yield return new JsonSchema().Type(JsonSchemaType.Number).Maximum(10);
			}
		} 
		
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsErrorOnInvalid(JsonSchema schema)
		{
			var json = new JsonArray();

			var results = schema.Validate(json);

			Assert.AreEqual(false, results.IsValid);
		}
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsValid(JsonSchema schema)
		{
			var json = (JsonValue) 5;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}