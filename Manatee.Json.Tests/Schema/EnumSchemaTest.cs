using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class EnumSchemaTest
	{
		public static IEnumerable TestData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema06
					{
						Enum = new List<EnumSchemaValue>
							{
								"test1",
								"test2"
							}
					});
				yield return new TestCaseData(new JsonSchema06
					{
						Enum = new List<EnumSchemaValue>
							{
								"test1",
								"test2"
							}
					});
			}
		}

		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsErrorOnValueOutOfRange(IJsonSchema schema)
		{
			var json = (JsonValue) "string";

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}
		[TestCaseSource(nameof(TestData))]
		public void ValidateReturnsValidOnValueInRange(IJsonSchema schema)
		{
			var json = (JsonValue) "test1";

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
