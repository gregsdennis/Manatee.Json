using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Path;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class ClientTest
	{
		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
		public void SerializerTemplateGeneration_EnumWithoutNamedZero_Danoceline_Issue13()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			var expected = "0";

			var actual = serializer.GenerateTemplate<NoNamedZero>();

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DeserializeUnnamedEnumEtry_InspiredBy_Danoceline_Issue13()
		{
			var serializer = new JsonSerializer { Options = { EnumSerializationFormat = EnumSerializationFormat.AsName } };
			JsonValue json = "10";
			var expected = (NoNamedZero) 10;

			var actual = serializer.Deserialize<NoNamedZero>(json);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DeserializeSchema_TypePropertyIsArray_Issue14()
		{
			var text = "{\"type\":\"object\",\"properties\":{\"home\":{\"type\":[\"object\",\"null\"],\"properties\":{\"street\":{\"type\":\"string\"}}}}}";
			var json = JsonValue.Parse(text);
			var expected = new ObjectSchema
				{
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition("home")
								{
									Type = new MultiSchema(new ObjectSchema
										{
											Properties = new JsonSchemaPropertyDefinitionCollection
												{
													new JsonSchemaPropertyDefinition("street")
														{
															Type = new StringSchema()
														}
												}
										},
									                       new NullSchema())
								}
						}
				};

			var actual = JsonSchemaFactory.FromJson(json);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DeclaredTypeWithDeclaredEnum_Issue15()
		{
			var text = "{\"type\":\"string\",\"enum\":[\"FeatureCollection\"]}";
			var json = JsonValue.Parse(text);
			var expected = new EnumSchema
				{
					Type = JsonSchemaTypeDefinition.String,
					Values = new List<EnumSchemaValue>
						{
							new EnumSchemaValue("FeatureCollection")
						}
				};

			var actual = JsonSchemaFactory.FromJson(json);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void PathIntolerantOfUndscoresInPropertyNames_Issue16()
		{
			var expected = JsonPathWith.Name("properties").Name("id_person");

			var actual = JsonPath.Parse("$.properties.id_person");

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		[TestMethod]
		public void UriValidationIntolerantOfLocalHost_Issue17()
		{
			var text = "{\"id\": \"http://localhost/json-schemas/address.json\"}";
			var json = JsonValue.Parse(text);

			var results = JsonSchema.Draft04.Validate(json);

			Assert.AreEqual(0, results.Errors.Count());
			Assert.AreEqual(true, results.Valid);
		}
	}
}
