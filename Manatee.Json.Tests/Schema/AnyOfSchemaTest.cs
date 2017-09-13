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
						AnyOf = new List<IJsonSchema>
							{
								new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array},
								new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number}
							}
					});
				yield return new TestCaseData(new JsonSchema06
					{
						AnyOf = new List<IJsonSchema>
							{
								new JsonSchema06 {Type = JsonSchemaTypeDefinition.Array},
								new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number}
							}
					});
			}
		}
		[TestCaseSource(nameof(ValidateReturnsErrorOnNoneValidData))]
		public void ValidateReturnsErrorOnNoneValid(IJsonSchema schema)
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
						AnyOf = new List<IJsonSchema>
							{
								new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number,Minimum = 10},
								new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number,Maximum = 20}
							}
					});
				yield return new TestCaseData(new JsonSchema06
					{
						AnyOf = new List<IJsonSchema>
							{
								new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number,Minimum = 10},
								new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number,Maximum = 20}
							}
					});
			}
		}
		[TestCaseSource(nameof(ValidateReturnsValidOnSingleValidData))]
		public void ValidateReturnsValidOnSingleValid(IJsonSchema schema)
		{
			var json = (JsonValue) 25;

			var results = schema.Validate(json);

			results.AssertValid();
		}
	}
}