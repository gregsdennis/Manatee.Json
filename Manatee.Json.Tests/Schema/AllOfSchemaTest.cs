using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
				yield return new TestCaseData(new JsonSchema04
					{
						AllOf = new List<IJsonSchema>
							{
								new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array},
								new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number}
							}
					});
				yield return new TestCaseData(new JsonSchema06
					{
						AllOf = new List<IJsonSchema>
							{
								new JsonSchema06 {Type = JsonSchemaTypeDefinition.Array},
								new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number}
							}
					});
			}
		}
		[TestCaseSource(nameof(ValidateReturnsErrorOnAnyInvalidData))]
		public void ValidateReturnsErrorOnAnyInvalid(IJsonSchema schema)
		{
			var json = new JsonObject();

			var results = schema.Validate(json);

			Assert.AreNotEqual(0, results.Errors.Count());
			Assert.AreEqual(false, results.Valid);
		}

		public static IEnumerable ValidateReturnsValidOnAllValidData
		{
			get
			{
				yield return new TestCaseData(new JsonSchema04
					{
						AllOf = new List<IJsonSchema>
							{
								new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number,Minimum = 10},
								new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number,Maximum = 20}
							}
					});
				yield return new TestCaseData(new JsonSchema06
					{
						AllOf = new List<IJsonSchema>
							{
								new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number,Minimum = 10},
								new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number,Maximum = 20}
							}
					});
			}
		}
		[TestCaseSource(nameof(ValidateReturnsValidOnAllValidData))]
		public void ValidateReturnsValidOnAllValid(IJsonSchema schema)
		{
			var json = (JsonValue) 15;

			var results = schema.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}