using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ClientTests
	{
		public static IEnumerable Issue167TestCaseSource
		{
			get
			{
				yield return new JsonSchema04
					{
						Properties = new Dictionary<string, IJsonSchema>
							{
								["xyz"] = new JsonSchema04
									{
										Type = JsonSchemaType.Object,
										Properties = new Dictionary<string, IJsonSchema>
											{
												["A"] = new JsonSchema04 {Type = JsonSchemaType.String},
												["B"] = new JsonSchema04 {Type = JsonSchemaType.Integer},
												["C"] = new JsonSchema04 {Type = JsonSchemaType.Number},
											},
										Required = new[] {"A"},
										AdditionalProperties = AdditionalProperties.False,
										OneOf = new IJsonSchema[]
											{
												new JsonSchema04 {Required = new[] {"B"}},
												new JsonSchema04 {Required = new[] {"C"}}
											}
									}
							}
					};
				yield return new JsonSchema06
					{
						Properties = new Dictionary<string, IJsonSchema>
							{
								["xyz"] = new JsonSchema06
									{
										Type = JsonSchemaType.Object,
										Properties = new Dictionary<string, IJsonSchema>
											{
												["A"] = new JsonSchema06 {Type = JsonSchemaType.String},
												["B"] = new JsonSchema06 {Type = JsonSchemaType.Integer},
												["C"] = new JsonSchema06 {Type = JsonSchemaType.Number},
											},
										Required = new[] {"A"},
										AdditionalProperties = (JsonSchema06)false,
										OneOf = new IJsonSchema[]
											{
												new JsonSchema06 {Required = new[] {"B"}},
												new JsonSchema06 {Required = new[] {"C"}}
											}
									}
							}
					};
				yield return new JsonSchema07
					{
						Properties = new Dictionary<string, IJsonSchema>
							{
								["xyz"] = new JsonSchema07
									{
										Type = JsonSchemaType.Object,
										Properties = new Dictionary<string, IJsonSchema>
											{
												["A"] = new JsonSchema07 { Type = JsonSchemaType.String },
												["B"] = new JsonSchema07 { Type = JsonSchemaType.Integer },
												["C"] = new JsonSchema07 { Type = JsonSchemaType.Number },
											},
										Required = new[] { "A" },
										AdditionalProperties = (JsonSchema07)false,
										OneOf = new IJsonSchema[]
											{
												new JsonSchema07 {Required = new[] {"B"}},
												new JsonSchema07 {Required = new[] {"C"}}
											}
									}
							}
					};
			}
		}

		[Test]
		[TestCaseSource(nameof(Issue167TestCaseSource))]
		public void Issue167_OneOfWithRequiredShouldFailValidation(IJsonSchema schema)
		{
			var json = new JsonObject
				{
					["xyz"] = new JsonObject
						{
							["A"] = "abc"
						}
				};

			var results = schema.Validate(json);

			Assert.IsFalse(results.Valid);
			Console.WriteLine(string.Join("\n", results.Errors));
		}
	}
}
