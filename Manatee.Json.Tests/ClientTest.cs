using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Manatee.Json.Path;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Path;
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
			var text = "{\"type\":\"object\",\"properties\":{\"home\":{\"type\":[\"object\",\"null\"],\"properties\":{\"street\":{\"type\":\"string\"}}}}}";
			var json = JsonValue.Parse(text);
			var expected = new JsonSchema04
				{
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("home")
								{
									Type = new JsonSchema04
										{
											Type = new JsonSchemaMultiTypeDefinition(JsonSchemaTypeDefinition.Object, JsonSchemaTypeDefinition.Null),
											Properties = new JsonSchemaPropertyDefinitionCollection
												{
													new JsonSchemaPropertyDefinition("street")
														{
															Type = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
														}
												}
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
			var text = "{\"type\":\"string\",\"enum\":[\"FeatureCollection\"]}";
			var json = JsonValue.Parse(text);
			var expected = new JsonSchema04
				{
					Type = JsonSchemaTypeDefinition.String,
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

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
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
			Assert.IsTrue(result.Valid);
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
			var refSchema = ((JsonSchemaReference)((JsonSchema04)schema).Properties["prop2"].Type).Resolved;
			Console.WriteLine(refSchema.ToJson(null));
			Console.WriteLine(json);
			foreach (var error in result.Errors)
			{
				Console.WriteLine(error);
			}

			Assert.IsTrue(result.Valid);
		}

		[Test]
		public void Issue49_RequiredAndAllOfInSingleSchema()
		{
			var fileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\issue49.json").AdjustForOS();
			var expected = new JsonSchema04
				{
					Title = "JSON schema for Something",
					Schema = "http://json-schema.org/draft-04/schema#",
					Definitions = new JsonSchemaTypeDefinitionCollection
						{
							new JsonSchemaTypeDefinition("something")
								{
									Definition = new JsonSchema04
										{
											Type = JsonSchemaTypeDefinition.Object,
											Properties = new JsonSchemaPropertyDefinitionCollection
												{
													new JsonSchemaPropertyDefinition("name")
														{
															IsHidden = true,
															IsRequired = true
														}
												},
											AllOf = new[]
												{
													new JsonSchema04
														{
															Properties = new JsonSchemaPropertyDefinitionCollection
																{
																	new JsonSchemaPropertyDefinition("name")
																		{
																			Type = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String}
																		}
																}
														}
												}
										}
								}
						},
					Type = JsonSchemaTypeDefinition.Array,
					Description = "An array of somethings.",
					Items = new JsonSchemaReference(new JsonSchema04(), "#/definitions/something")
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
			Assert.IsTrue(results.Valid);
		}

		[Test]
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

			Assert.IsTrue(result.Valid);
			Assert.AreEqual(requestedUris[0], childSchemaUri);
			Assert.AreEqual(requestedUris[1], coreSchemaUri);
		}

	}
}
