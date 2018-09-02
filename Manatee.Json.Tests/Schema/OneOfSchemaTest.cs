using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
				yield return new JsonSchema04
					{
						OneOf = new List<JsonSchema>
							{
								new JsonSchema04 {Type = JsonSchemaType.Number, Minimum = 5},
								new JsonSchema04 {Type = JsonSchemaType.Number, Minimum = 10}
							}
					};
				yield return new JsonSchema06
					{
						OneOf = new List<JsonSchema>
							{
								new JsonSchema06 {Type = JsonSchemaType.Number, Minimum = 5},
								new JsonSchema06 {Type = JsonSchemaType.Number, Minimum = 10}
							}
					};
				yield return new JsonSchema07
				{
						OneOf = new List<JsonSchema>
							{
								new JsonSchema07 {Type = JsonSchemaType.Number, Minimum = 5},
								new JsonSchema07 {Type = JsonSchemaType.Number, Minimum = 10}
							}
					};
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