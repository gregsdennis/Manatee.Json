using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Manatee.Json.Pointer;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class ClientTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			JsonOptions.LogCategory = LogCategory.Schema;
		}

		private static readonly JsonSerializer _serializer = new JsonSerializer();

		[Test]
		public void Issue15_DeclaredTypeWithDeclaredEnum()
		{
			var text = "{\"type\":\"string\",\"enum\":[\"FeatureCollection\"]}";
			var json = JsonValue.Parse(text);
			var expected = new JsonSchema()
				.Type(JsonSchemaType.String)
				.Enum("FeatureCollection");

			var actual = new JsonSchema();
			actual.FromJson(json, _serializer);

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

			foreach (var error in result.NestedResults)
			{
				Console.WriteLine(error);
			}
			Console.WriteLine(schema.ToJson(_serializer));
			var refSchema = schema.Properties()["prop2"].RefResolved();
			Console.WriteLine(refSchema.ToJson(_serializer));
			Console.WriteLine(json);

			result.AssertValid();
		}

		[Test]
		public void Issue49_RequiredAndAllOfInSingleSchema()
		{
			var fileName = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, @"Files\issue49.json").AdjustForOS();
			var expected = new JsonSchema()
				.Title("JSON schema for Something")
				.Schema("http://json-schema.org/draft-04/schema#")
				.Definition("something", new JsonSchema()
					            .Type(JsonSchemaType.Object)
					            .Required("name")
					            .AllOf(new JsonSchema()
						                   .Property("name", new JsonSchema().Type(JsonSchemaType.String)))
				)
				.Type(JsonSchemaType.Array)
				.Description("An array of somethings.")
				.Items(new JsonSchema().Ref("#/definitions/something"));

			var schema = JsonSchemaRegistry.Get(fileName);

			Assert.AreEqual(expected, schema);

			var schemaJson = schema.ToJson(_serializer);
			var expectedJson = expected.ToJson(_serializer);

			Console.WriteLine(schemaJson);
			Assert.AreEqual(expectedJson, schemaJson);
		}

		[Test]
		public void Issue50_MultipleSchemaInSubFoldersShouldReferenceRelatively()
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
			var schema = new JsonSchema()
				.Schema(MetaSchemas.Draft06.Id)
				.Type(JsonSchemaType.Object)
				.Property("test", new JsonSchema()
					          .Id("/properties/test")
					          .Type(JsonSchemaType.String)
					          .Title("Test property")
					          .Description("Test property")
					          .Default("")
					          .Examples("any string"));
			var json = new JsonObject {["test"] = "a valid string"};

			var results = schema.Validate(json);

			Console.WriteLine(string.Join("\n", results.NestedResults));

			Assert.IsTrue(results.IsValid);
		}

		[Test]
		public void Issue167_OneOfWithRequiredShouldFailValidation()
		{
			var schema = new JsonSchema()
				.Property("xyz", new JsonSchema()
					          .Type(JsonSchemaType.Object)
					          .Property("A", new JsonSchema().Type(JsonSchemaType.String))
					          .Property("B", new JsonSchema().Type(JsonSchemaType.Integer))
					          .Property("C", new JsonSchema().Type(JsonSchemaType.Number))
					          .Required("A")
					          .AdditionalProperties(false)
					          .OneOf(new JsonSchema().Required("B"),
					                 new JsonSchema().Required("C")));
			var json = new JsonObject
				{
					["xyz"] = new JsonObject
						{
							["A"] = "abc"
						}
				};

			var results = schema.Validate(json);

			Assert.IsFalse(results.IsValid);
			Console.WriteLine(string.Join("\n", results.NestedResults));
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
			var schema = new JsonSchema()
				.Schema(MetaSchemas.Draft06.Id)
				.Definition("fields", new JsonSchema()
					            .Type(JsonSchemaType.Object)
					            .Property("field1", new JsonSchema().Type(JsonSchemaType.String))
					            .Property("field2", new JsonSchema().Type(JsonSchemaType.Integer)))
				.Definition("xyzBaseFieldNames", new JsonSchema()
					            .Enum("field1", "field2"))
				.Definition("worldwide", new JsonSchema()
					            .AllOf(new JsonSchema().Ref("#/definitions/fields"),
					                   new JsonSchema().Required("field1"),
					                   new JsonSchema()
						                   .Property("A", new JsonSchema().Type(JsonSchemaType.String))
						                   .Property("B", new JsonSchema().Type(JsonSchemaType.Integer))
						                   .OneOf(new JsonSchema().Required("A"),
						                          new JsonSchema().Required("B")),
					                   new JsonSchema()
						                   .PropertyNames(new JsonSchema()
							                                  .AnyOf(new JsonSchema().Ref("#/definitions/xyzBaseFieldNames"),
							                                         new JsonSchema().Enum("A", "B")))))
				.Type(JsonSchemaType.Object)
				.Property("xyz", new JsonSchema().Ref("#/definitions/worldwide"))
				.AdditionalProperties(false)
				.Required("xyz");

			var results = schema.Validate(json);

			Assert.AreEqual(isValid, results.IsValid);
		}

		[Test]
		public void Issue173_ReferencedSchemaInParentFolder()
		{
			try
			{
				// this links to the commit after the one that submitted the schema files.
				// otherwise we have a paradox of trying to know the commit hash before the commit is created.
				var baseUri = "https://raw.githubusercontent.com/gregsdennis/Manatee.Json/c264db5c75478e0a33269baba7813901829f8244/Manatee.Json.Tests/Files/";

				var schema = JsonSchemaRegistry.Get($"{baseUri}Issue173/BaseSchema.json");

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
			catch (WebException)
			{
				Assert.Inconclusive();
			}
			catch (AggregateException e)
			{
				if (e.InnerExceptions.OfType<WebException>().Any() ||
				    e.InnerExceptions.OfType<HttpRequestException>().Any())
					Assert.Inconclusive();
				throw;
			}
		}

		[Test]
		public void Issue194_refNoIntuitiveErrorMessage()
		{
			var actual = new JsonSchema()
				.Ref("#/definitions/apredefinedtype")
				.Definition("apredefinedtype", new JsonSchema()
					            .Type(JsonSchemaType.Object)
					            .Property("prop1", new JsonSchema().Enum("ændring", "test"))
					            .Required("prop1"));
			var jObject = new JsonObject
			{
				["prop11"] = "ændring",
				["prop2"] = new JsonObject { ["prop3"] = "ændring" }
			};
			var expected = new SchemaValidationResults
				{
					IsValid = false,
					RelativeLocation = JsonPointer.Parse("#/$ref/required"),
					InstanceLocation = JsonPointer.Parse("#"),
					Keyword = "required",
					ErrorMessage = "The properties [\"prop1\"] are required.",
					AdditionalInfo = new JsonObject
						{
							["properties"] = new JsonArray {"prop1"}

						}
				};

			var messages = actual.Validate(jObject, new JsonSchemaOptions {OutputFormat = SchemaValidationOutputFormat.Detailed});

			messages.AssertInvalid(expected);
		}

		[Test]
		public void Issue205_PropertiesEquality()
		{
			var schema1 = new JsonSchema()
				.Schema(MetaSchemas.Draft07.Id)
				.Property("foo", new JsonSchema().Type(JsonSchemaType.Integer))
				.Property("bar", new JsonSchema().Type(JsonSchemaType.String))
				.AdditionalProperties(false);
			var schema2 = new JsonSchema()
				.Schema(MetaSchemas.Draft07.Id)
				.Property("foo", new JsonSchema().Type(JsonSchemaType.Integer))
				.Property("bar", new JsonSchema().Type(JsonSchemaType.String))
				.Property("baz", new JsonSchema().Type(JsonSchemaType.Boolean))
				.AdditionalProperties(false);

			Assert.AreNotEqual(schema1, schema2);
		}

		[Test]
		public void Issue217_MaxLengthDoesNotSupportUtf8Properly()
		{
			var json = new JsonObject {["data"] = "“Silly Gooseberry!”"};
			var schema = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("data", new JsonSchema()
					          .MaxLength(20));

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void Issue219_RootSchemaIdentifiedByRelativeUri()
		{
			var schema = new JsonSchema()
				.Id("1")
				.Property("value", new JsonSchema()
					          .Type(JsonSchemaType.Integer))
				.Property("loop", new JsonSchema()
					          .Ref("1"))
				.Required("value");

			var json = new JsonObject
				{
					["value"] = 6,
					["loop"] = new JsonObject { ["value"] = 31 }
				};

			var results = schema.Validate(json);

			results.AssertValid();
		}

		[Test]
		public void Issue219_RootSchemaIdentifiedByRelativeUriFailsBecauseRefDoesNotUseFragment()
		{
			var serializer = new JsonSerializer();

			var schemaText =
				@"{""$id"":""1"",""definitions"":{""defaultTemplate"":{""$id"":""template"",""type"":""object"",""required"":[""movie""],""properties"":{""movie"":{""type"":""object""}}}},""$schema"":""http:\/\/json-schema.org\/draft-07\/schema"",""allOf"":[{""$ref"":""/definitions/defaultTemplate""},{""required"":[""movie""],""properties"":{""movie"":{""required"":[""metadata""],""properties"":{""metadata"":{""type"":""object"",""required"":[""name"",""movieId""],""properties"":{""name"":{""type"":""string""},""movieId"":{""type"":""string""}}}}}}}]}";
			var schemaJson = JsonValue.Parse(schemaText);
			var schema = serializer.Deserialize<JsonSchema>(schemaJson);

			var jsonText = @"{ ""movie"": { ""metadata"": { ""name"": ""1"", ""movieId"": ""12"" } } }";
			var json = JsonValue.Parse(jsonText);

			Assert.Throws<SchemaReferenceNotFoundException>(() => schema.Validate(json));
		}

		[Test]
		public void Issue231_ReferenceNotFoundExceptionOnValidRef()
		{
			var schema = new JsonSchema()
				.Schema(MetaSchemas.Draft07.Id)
				.Id("http://localhost/Resistance.schema.json")
				.Type(JsonSchemaType.Object)
				.Definition("1DVector", new JsonSchema()
					            .Id("#1DVector")
					            .Type(JsonSchemaType.Array)
					            .MinItems(1)
					            .Items(new JsonSchema()
						                   .AnyOf(new JsonSchema().Type(JsonSchemaType.Number),
						                          new JsonSchema().Type(JsonSchemaType.Null))
						                   .Default(JsonValue.Null))
					            .Examples(1.1, 2.3, 4.5))
				.Definition("t_PulseTimePeriod", new JsonSchema()
					            .Id("#t_PulseTimePeriod")
					            .Type(JsonSchemaType.Object)
					            .Title("The t_PulseTimePeriod Schema")
					            .Required("values")
					            .Property("values", new JsonSchema().Ref("#/definitions/1DVector")))
				.Required("t_PulseTimePeriod")
				.Property("t_PulseTimePeriod", new JsonSchema().Ref("#t_PulseTimePeriod"))
				.AdditionalProperties(false);

			var instance = new JsonObject {["t_PulseTimePeriod"] = new JsonObject {["values"] = new JsonArray()}};

			var result = schema.Validate(instance, new JsonSchemaOptions { OutputFormat = SchemaValidationOutputFormat.Detailed });

			result.AssertInvalid();
		}

		[Test]
		public void Issue232_RefsNotResolvedAgainstId()
		{
			var resistanceSchema = new JsonSchema()
				.Schema(MetaSchemas.Draft07.Id)
				.Id("http://localhost/Resistance.schema.json")
				.Type(JsonSchemaType.Object)
				.Required("t_PulseTimePeriod")
				.Property("t_PulseTimePeriod", new JsonSchema().Ref("./definitions.schema.json#definitions/t_PulseTimePeriod"))
				.AdditionalProperties(false);
			var definitionsSchema = new JsonSchema()
				.Schema(MetaSchemas.Draft07.Id)
				.Id("http://localhost/definitions.schema.json")
				.Definition("1DVector", new JsonSchema()
					            .Id("#1DVector")
					            .Type(JsonSchemaType.Array)
					            .MinItems(1)
					            .Items(new JsonSchema()
						                   .AnyOf(new JsonSchema().Type(JsonSchemaType.Number),
						                          new JsonSchema().Type(JsonSchemaType.Null))
						                   .Default(JsonValue.Null))
					            .Examples(1.1, 2.3, 4.5))
				.Definition("t_PulseTimePeriod", new JsonSchema()
					            .Id("#t_PulseTimePeriod")
					            .Type(JsonSchemaType.Object)
					            .Title("The t_PulseTimePeriod Schema")
					            .Required("values")
					            .Property("values", new JsonSchema().Ref("#/definitions/1DVector")));

			JsonSchemaRegistry.Register(resistanceSchema);
			JsonSchemaRegistry.Register(definitionsSchema);

			var instance = new JsonObject { ["t_PulseTimePeriod"] = new JsonObject { ["values"] = new JsonArray() } };

			var result = resistanceSchema.Validate(instance, new JsonSchemaOptions { OutputFormat = SchemaValidationOutputFormat.Detailed });

			result.AssertInvalid();
		}

		[Test]
		public void Issue248_RefNotPassingEvaluatedProperties()
		{
			var schema = new JsonSchema()
				.Definition("other", new JsonSchema()
								.Type(JsonSchemaType.Object)
								.Property("surfboard", new JsonSchema().Type(JsonSchemaType.String)))
				.AllOf(new JsonSchema().Ref("#/definitions/other"),
					   new JsonSchema()
						   .Property("wheels", true)
						   .Property("headlights", true),
					   new JsonSchema().Property("pontoons", true),
					   new JsonSchema().Property("wings", true))
				.UnevaluatedProperties(false);

			var instance = new JsonObject
				{
					["pontoons"] = new JsonObject(),
					["wheels"] = new JsonObject(),
					["surfboard"] = "2"
				};

			var results = schema.Validate(instance);

			results.AssertValid();
		}

		[TestCase("draft7", 1)]
		[TestCase("draft2019-09", 2)]
		public void Issue269_DownloadOverrideNotWorking(string draft, int expectedDownloadCount)
		{
			try
			{
				var schemaDirPath = $"{AppDomain.CurrentDomain.BaseDirectory}/Files/Issue269/{draft}/";
				var schemaUriPrefix = "http://localhost/";
				var jsonData = JsonValue.Parse("{\"propertyAffectedFromSubSchema\": {}}");
				var serializer = new JsonSerializer();
				var downloadCount = 0;

				JsonSchemaOptions.Download = uri =>
					{
						downloadCount++;
						Console.WriteLine($"Downloading {uri}");
						var localSchemaPath = uri.Replace(schemaUriPrefix, schemaDirPath); //Set Breakpoint
						var filePath = $"{localSchemaPath}.schema.json";
						return File.ReadAllText(filePath);
					};

				JsonSchemaRegistry.Clear();
				var instanceSchemaFilePath = $"{System.IO.Path.Combine(schemaDirPath, "schemas/1.1.0/instance")}.schema.json";
				var schemaJson = JsonValue.Parse(File.ReadAllText(instanceSchemaFilePath));
				var schema = serializer.Deserialize<JsonSchema>(schemaJson);
				var validationResults = schema.Validate(jsonData);

				Assert.AreEqual(expectedDownloadCount, downloadCount);
			}
			finally
			{
				JsonSchemaOptions.Download = null!;
			}
		}
	}
}
