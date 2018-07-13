using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ClientTests
	{
		[Test]
		public void Issue15_DeclaredTypeWithDeclaredEnum()
		{
			JsonSchemaFactory.SetDefaultSchemaVersion<JsonSchema04>();

			var text = "{\"type\":\"string\",\"enum\":[\"FeatureCollection\"]}";
			var json = JsonValue.Parse(text);
			var expected = new JsonSchema04
			{
				Type = JsonSchemaType.String,
				Enum = new List<EnumSchemaValue>
						{
							new EnumSchemaValue("FeatureCollection")
						}
			};

			var actual = JsonSchemaFactory.FromJson(json);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Issue45a_Utf8SupportInReferenceSchemaEnums()
		{
			var fileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\baseSchema.json").AdjustForOS();

			const string jsonString = "{\"prop1\": \"ændring\", \"prop2\": {\"prop3\": \"ændring\"}}";
			var schema = JsonSchemaRegistry.Get(fileName);
			var json = JsonValue.Parse(jsonString);

			var result = schema.Validate(json);

			result.AssertValid();
		}

		[Test]
		public void Issue45b_Utf8SupportInReferenceSchemaEnums()
		{
			var fileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\baseSchema.json").AdjustForOS();

			const string jsonString = "{\"prop1\": \"ændring\", \"prop2\": {\"prop3\": \"ændring\"}}";
			var schema = JsonSchemaRegistry.Get(fileName);
			var json = JsonValue.Parse(jsonString);

			var result = schema.Validate(json);

			Console.WriteLine(schema.ToJson(null));
			var refSchema = ((JsonSchemaReference)((JsonSchema04)schema).Properties["prop2"]).Resolved;
			Console.WriteLine(refSchema.ToJson(null));
			Console.WriteLine(json);
			foreach (var error in result.Errors)
			{
				Console.WriteLine(error);
			}

			result.AssertValid();
		}

		[Test]
		public void Issue49_RequiredAndAllOfInSingleSchema()
		{
			var fileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\issue49.json").AdjustForOS();
			var expected = new JsonSchema04
			{
				Title = "JSON schema for Something",
				Schema = "http://json-schema.org/draft-04/schema#",
				Definitions = new Dictionary<string, IJsonSchema>
				{
					["something"] = new JsonSchema04
					{
						Type = JsonSchemaType.Object,
						AllOf = new[]
										{
											new JsonSchema04
												{
													Properties = new Dictionary<string, IJsonSchema>
														{
															["name"] = new JsonSchema04 {Type = JsonSchemaType.String}
														}
												}
										},
						Required = new List<string> { "name" }
					}
				},
				Type = JsonSchemaType.Array,
				Description = "An array of somethings.",
				Items = new JsonSchemaReference("#/definitions/something", typeof(JsonSchema04))
			};

			var schema = JsonSchemaRegistry.Get(fileName);

			Assert.AreEqual(expected, schema);

			var schemaJson = schema.ToJson(null);
			var expectedJson = expected.ToJson(null);

			Console.WriteLine(schemaJson);
			Assert.AreEqual(expectedJson, schemaJson);
		}

		[Test]
		public void Issue50_MulitpleSchemaInSubFoldersShouldReferenceRelatively()
		{
			string path = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\Issue50A.json").AdjustForOS();
			var schema = JsonSchemaRegistry.Get(path);
			var json = new JsonObject
			{
				["text"] = "something",
				["refa"] = new JsonObject
				{
					["text"] = "something else",
					["refb"] = new JsonObject
					{
						["refd"] = new JsonObject
						{
							["refe"] = new JsonObject { ["test"] = "test" },
							["text"] = "test"
						}
					}
				}
			};
			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void Issue58_UriReferenceSchemaTest()
		{
			const string coreSchemaUri = "http://example.org/Issue58RefCore.json";
			const string childSchemaUri = "http://example.org/Issue58RefChild.json";

			var coreSchemaPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\Issue58RefCore.json").AdjustForOS();
			var childSchemaPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\Issue58RefChild.json").AdjustForOS();

			string coreSchemaText;
			string childSchemaText;

			using (TextReader reader = File.OpenText(coreSchemaPath))
			{
				coreSchemaText = reader.ReadToEnd();
			}

			using (TextReader reader = File.OpenText(childSchemaPath))
			{
				childSchemaText = reader.ReadToEnd();
			}

			var requestedUris = new List<string>();
			try
			{
				JsonSchemaOptions.Download = uri =>
				{
					requestedUris.Add(uri);
					switch (uri)
					{
						case coreSchemaUri:
							return coreSchemaText;

						case childSchemaUri:
							return childSchemaText;
					}
					return coreSchemaText;
				};
				var schema = JsonSchemaRegistry.Get(childSchemaUri);

				var testJson = new JsonObject();
				testJson["myProperty"] = "http://example.org/";

				//Console.WriteLine(testJson);
				//Console.WriteLine(schema.ToJson(null).GetIndentedString());

				var result = schema.Validate(testJson);

				result.AssertValid();
				Assert.AreEqual(requestedUris[0], childSchemaUri);
				Assert.AreEqual(requestedUris[1], coreSchemaUri);
			}
			finally
			{
				JsonSchemaOptions.Download = null;
			}
		}

		[Test]
		public void Issue141_ExamplesInSchema()
		{
			var schema = new JsonSchema06
			{
				Schema = JsonSchema06.MetaSchema.Id,
				Type = JsonSchemaType.Object,
				Properties = new Dictionary<string, IJsonSchema>
				{
					["test"] = new JsonSchema06
					{
						Id = "/properties/test",
						Type = JsonSchemaType.String,
						Title = "Test property",
						Description = "Test property",
						Default = "",
						Examples = new JsonArray { "any string" }
					}
				}
			};
			var json = new JsonObject { ["test"] = "a valid string" };

			var results = schema.Validate(json);

			Console.WriteLine(string.Join("\n", results.Errors));

			Assert.IsTrue(results.Valid);
		}

		public static IEnumerable Issue167TestCaseSource1
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
		[TestCaseSource(nameof(Issue167TestCaseSource1))]
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

		public static IEnumerable Issue167TestCaseSource2
		{
			get
			{
				yield return new TestCaseData(new JsonObject
					                              {
						                              ["xyz"] = new JsonObject
							                              {
								                              ["field1"] = "abc",
								                              ["field2"] = 1,
								                              ["A"] = "def"
							                              }
					                              },
				                              true);
				yield return new TestCaseData(new JsonObject
					                              {
						                              ["xyz"] = new JsonObject
							                              {
								                              ["field1"] = "abc",
								                              ["A"] = "def"
							                              }
					                              },
				                              true);
				yield return new TestCaseData(new JsonObject
					                              {
						                              ["xyz"] = new JsonObject
							                              {
								                              ["field1"] = "abc",
								                              ["field2"] = 1,
								                              ["A"] = "def",
								                              ["B"] = 3
							                              }
					                              },
				                              false);
				yield return new TestCaseData(new JsonObject
					                              {
						                              ["xyz"] = new JsonObject
							                              {
								                              ["A"] = "def"
							                              }
					                              },
				                              false);
				yield return new TestCaseData(new JsonObject
					                              {
						                              ["xyz"] = new JsonObject
							                              {
								                              ["field2"] = 1,
								                              ["A"] = "def"
							                              }
					                              },
				                              false);
			}
		}

		[Test]
		[TestCaseSource(nameof(Issue167TestCaseSource2))]
		public void Issue167_PropertyNamesWithPropertylessRequired(JsonObject json, bool isValid)
		{
			var schema = new JsonSchema06
				{
					Schema = JsonSchema06.MetaSchema.Id,
					Definitions = new Dictionary<string, IJsonSchema>
						{
							["fields"] = new JsonSchema06
								{
									Type = JsonSchemaType.Object,
									Properties = new Dictionary<string, IJsonSchema>
										{
											["field1"] = new JsonSchema06 {Type = JsonSchemaType.String},
											["field2"] = new JsonSchema06 {Type = JsonSchemaType.Integer}
										}
								},
							["xyzBaseFieldNames"] = new JsonSchema06
								{
									Enum = new EnumSchemaValue[] {"field1", "field2"}
								},
							["worldwide"] = new JsonSchema06
								{
									AllOf = new IJsonSchema[]
										{
											new JsonSchemaReference("#/definitions/fields", typeof(JsonSchema06)),
											new JsonSchema06 {Required = new[] {"field1"}},
											new JsonSchema06
												{
													Properties = new Dictionary<string, IJsonSchema>
														{
															["A"] = new JsonSchema06 {Type = JsonSchemaType.String},
															["B"] = new JsonSchema06 {Type = JsonSchemaType.Integer}
														},
													OneOf = new IJsonSchema[]
														{
															new JsonSchema06 {Required = new[] {"A"}},
															new JsonSchema06 {Required = new[] {"B"}},
														}
												},
											new JsonSchema06
												{
													PropertyNames = new JsonSchema06
														{
															AnyOf = new IJsonSchema[]
																{
																	new JsonSchemaReference("#/definitions/xyzBaseFieldNames", typeof(JsonSchema06)),
																	new JsonSchema06
																		{
																			Enum = new EnumSchemaValue[] {"A", "B"}
																		}
																}
														}
												}
										}
								}
						},
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, IJsonSchema>
						{
							["xyz"] = new JsonSchemaReference("#/definitions/worldwide", typeof(JsonSchema06))
						},
					AdditionalProperties = JsonSchema06.False,
					Required = new[] {"xyz"}
				};

			var results = schema.Validate(json);

			Assert.AreEqual(isValid, results.Valid);
		}

		[Test]
		public void Issue173_ReferencedSchemaInParentFolder()
		{
			var baseUri = "https://raw.githubusercontent.com/gregsdennis/Manatee.Json/bug/173-parent-folder-schema-ref/Manatee.Json.Tests/Files/";

			var schema = (JsonSchema07) JsonSchemaRegistry.Get($"{baseUri}Issue173/BaseSchema.json");

			var invalid = new JsonObject
				{
					["localProp"] = new JsonArray {150, "hello", 6}
				};
			var valid = new JsonObject
				{
					["localProp"] = new JsonArray {1, 2, 3, 4}
				};

			var result = schema.Validate(invalid);
			result.AssertInvalid();

			result = schema.Validate(valid);
			result.AssertValid();
		}
	}
}
