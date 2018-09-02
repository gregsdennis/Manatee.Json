using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
				yield return new TestCaseData(new JsonSchema04
					{
						AnyOf = new List<JsonSchema>
							{
								new JsonSchema04 {Type = JsonSchemaType.Array},
								new JsonSchema04 {Type = JsonSchemaType.Number}
							}
					});
				yield return new TestCaseData(new JsonSchema06
					{
						AnyOf = new List<JsonSchema>
							{
								new JsonSchema06 {Type = JsonSchemaType.Array},
								new JsonSchema06 {Type = JsonSchemaType.Number}
							}
					});
				yield return new TestCaseData(new JsonSchema07
					{
						AnyOf = new List<JsonSchema>
							{
								new JsonSchema07 {Type = JsonSchemaType.Array},
								new JsonSchema07 {Type = JsonSchemaType.Number}
							}
					});
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
				yield return new TestCaseData(new JsonSchema04
					{
						AnyOf = new List<JsonSchema>
							{
								new JsonSchema04 {Type = JsonSchemaType.Number, Minimum = 10},
								new JsonSchema04 {Type = JsonSchemaType.Number, Maximum = 20}
							}
					});
				yield return new TestCaseData(new JsonSchema06
					{
						AnyOf = new List<JsonSchema>
							{
								new JsonSchema06 {Type = JsonSchemaType.Number, Minimum = 10},
								new JsonSchema06 {Type = JsonSchemaType.Number, Maximum = 20}
							}
					});
				yield return new TestCaseData(new JsonSchema07
					{
						AnyOf = new List<JsonSchema>
							{
								new JsonSchema07 {Type = JsonSchemaType.Number, Minimum = 10},
								new JsonSchema07 {Type = JsonSchemaType.Number, Maximum = 20}
							}
					});
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