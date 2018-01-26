using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manatee.Json.Path;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Path;
using Manatee.Json.Tests.Schema;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;
// ReSharper disable EqualExpressionComparison
// ReSharper disable ExpressionIsAlwaysNull

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class ClientTest
	{
		[Test]
		public void Parse_StringFromSourceForge_kheimric()
		{
			var s = @"{
  ""self"": ""self"",
  ""name"": ""name"",
  ""emailAddress"": ""test at test dot com"",
  ""avatarUrls"": {
	""16x16"": ""http://smallUrl"",
	""48x48"": ""https://largeUrl""
  },
  ""displayName"": ""Display Name"",
  ""active"": true,
  ""timeZone"": ""Europe"",
  ""groups"": {
	""size"": 1,
	""items"": [
	  {
		""name"": ""users""
	  }
	]
  },
  ""expand"": ""groups""
}";
			JsonValue expected = new JsonObject
				{
					{"self", "self"},
					{"name", "name"},
					{"emailAddress", "test at test dot com"},
					{
						"avatarUrls", new JsonObject
							{
								{"16x16", "http://smallUrl"},
								{"48x48", "https://largeUrl"},
							}
					},
					{"displayName", "Display Name"},
					{"active", true},
					{"timeZone", "Europe"},
					{
						"groups", new JsonObject
							{
								{"size", 1},
								{
									"items", new JsonArray
										{
											new JsonObject {{"name", "users"}}
										}
								},
							}
					},
					{"expand", "groups"},
				};

			var actual = JsonValue.Parse(s);
			

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Parse_InternationalCharacters_DaVincious_TrelloIssue19()
		{
			var s = @"""ÅÄÖ""";
			var actual = JsonValue.Parse(s);
			var newString = actual.ToString();

			Console.WriteLine(s);
			Console.WriteLine(actual);
			Assert.AreEqual(s, newString);
		}

		public enum NoNamedZero
		{
			One = 1,
			Two = 2,
			Three = 3
		}

		[Test]
		public void SerializerTemplateGeneration_EnumWithoutNamedZero_Danoceline_Issue13()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			JsonValue expected = "0";

			var actual = serializer.GenerateTemplate<NoNamedZero>();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeUnnamedEnumEtry_InspiredBy_Danoceline_Issue13()
		{
			var serializer = new JsonSerializer { Options = { EnumSerializationFormat = EnumSerializationFormat.AsName } };
			JsonValue json = "10";
			var expected = (NoNamedZero) 10;

			var actual = serializer.Deserialize<NoNamedZero>(json);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeSchema_TypePropertyIsArray_Issue14()
		{
			JsonSchemaFactory.SetDefaultSchemaVersion<JsonSchema04>();
			
			var text = "{\"type\":\"object\",\"properties\":{\"home\":{\"type\":[\"object\",\"null\"],\"properties\":{\"street\":{\"type\":\"string\"}}}}}";
			var json = JsonValue.Parse(text);
			var expected = new JsonSchema04
				{
					Type = JsonSchemaType.Object,
					Properties = new Dictionary<string, IJsonSchema>
						{
							["home"] = new JsonSchema04
								{
									Type = JsonSchemaType.Object | JsonSchemaType.Null,
									Properties = new Dictionary<string, IJsonSchema>
										{
											["street"] = new JsonSchema04 {Type = JsonSchemaType.String}
										}
								}
						}
				};

			var actual = JsonSchemaFactory.FromJson(json);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeclaredTypeWithDeclaredEnum_Issue15()
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
		public void PathIntolerantOfUndscoresInPropertyNames_Issue16()
		{
			var expected = JsonPathWith.Name("properties").Name("id_person");

			var actual = JsonPath.Parse("$.properties.id_person");

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		[Test]
		public void UriValidationIntolerantOfLocalHost_Issue17()
		{
			var text = "{\"id\": \"http://localhost/json-schemas/address.json\"}";
			var json = JsonValue.Parse(text);

			var results = JsonSchema04.MetaSchema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void Issue31_JsonPathArrayOperatorShouldWorkOnObjects_Parsed()
		{
			var json = GoessnerExamplesTest.GoessnerData;
			var path = JsonPath.Parse(@"$.store.bicycle[?(1==1)]");

			var results = path.Evaluate(json);

			Assert.AreEqual(new JsonArray { "red", 19.95 }, results);
		}

		[Test]
		public void Issue31_JsonPathArrayOperatorShouldWorkOnObjects_Constructed()
		{
			var json = GoessnerExamplesTest.GoessnerData;
			var path = JsonPathWith.Name("store")
			                       .Name("bicycle")
			                       .Array(jv => 1 == 1);

			var results = path.Evaluate(json);

			Assert.AreEqual(new JsonArray { "red", 19.95 }, results);
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
			var refSchema = ((JsonSchemaReference) ((JsonSchema04) schema).Properties["prop2"]).Resolved;
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
									Required = new List<string>{"name"}
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
											["refe"] = new JsonObject{["test"] = "test"},
											["text"] = "test"
										}
								}
						}
				};
			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		[Ignore("This test no longer applies as of v9.1.0")]
		public void Issue56_InconsistentNullAssignment()
		{
			JsonValue json1 = null;  // this is actually null
			string myVar = null;
			JsonValue json2 = myVar;  // this is JsonValue.Null

			Assert.IsNull(json1);
			Assert.IsTrue(Equals(null, json1));

			Assert.IsNotNull(json2);
			Assert.IsTrue(null == json2);
			// R# isn't considering my == overload
			// ReSharper disable once HeuristicUnreachableCode
			Assert.IsTrue(json2.Equals(null));
			// This may seem inconsistent, but check out the notes in the issue.
			Assert.IsFalse(Equals(null, json2));
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
		public void Issue126_DictionarySerializationWithDefaultValuesExtendedExample()
		{
			var a = new Issue126
				{
					X = 1,
					Y = new Dictionary<string, bool>
						{
							{"t", true},
							{"f", false}, // ("f", false) != default((string, bool))
						},
					Z = new Dictionary<string, Issue126>
						{
							{"a", null}, // ("a", null) != default((string, A))
							{
								"b", new Issue126
									{
										X = 2,
										// Y is the default value for Dictionary<TKey,TValue>
										Z = new Dictionary<string, Issue126>
											{
												{"c", new Issue126 {X = 3}} // Y and Z are default values
											}
									}
							},
						},
				};

			JsonValue expected = new JsonObject
				{
					["X"] = 1,
					["Y"] = new JsonObject
						{
							["t"] = true,
							["f"] = false
						},
					["Z"] = new JsonObject
						{
							["a"] = null,
							["b"] = new JsonObject
								{
									["X"] = 2,
									["Z"] = new JsonObject
										{
											["c"] = new JsonObject
												{
													["X"] = 3

												}
										}
								}
						}
				};
			var serializer = new JsonSerializer();

			var actual = serializer.Serialize(a);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Issue130_SerializingStructsShouldNotProduceReferences()
		{
			var serializer = new JsonSerializer
				{
					Options =
						{
							PropertySelectionStrategy = PropertySelectionStrategy.ReadAndWrite
						}
				};
			JsonValue expected = new JsonObject
				{
					["A"] = "00:00:30",
					["B"] = "00:00:30"
				};

			var json = new {A = TimeSpan.FromSeconds(30), B = TimeSpan.FromSeconds(30)};
			var actual = serializer.Serialize(json);
			// produces {"A":"00:00:30","B":{"#Ref":"94e343ba-4ffd-4402-80be-67feb8299d90"}}

			Assert.AreEqual(expected, actual);
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
									Examples = new JsonArray {"any string"}
								}
						}
				};
			var json = new JsonObject {["test"] = "a valid string"};

			var results = schema.Validate(json);

			Console.WriteLine(string.Join("\n", results.Errors));

			Assert.IsTrue(results.Valid);
		}
	}
}
