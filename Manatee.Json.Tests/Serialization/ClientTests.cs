using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;

namespace Manatee.Json.Tests.Serialization
{
	[TestFixture]
	public class ClientTests
	{
		public enum NoNamedZero
		{
			One = 1,
			Two = 2,
			Three = 3
		}

		[Test]
		public void SerializerTemplateGeneration_EnumWithoutNamedZero_Danoceline_Issue13()
		{
			var serializer = new JsonSerializer { Options = { EnumSerializationFormat = EnumSerializationFormat.AsName } };
			JsonValue expected = "0";

			var actual = serializer.GenerateTemplate<NoNamedZero>();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeUnnamedEnumEntry_InspiredBy_Danoceline_Issue13()
		{
			var serializer = new JsonSerializer {Options = {EnumSerializationFormat = EnumSerializationFormat.AsName}};
			JsonValue json = "10";
			var expected = (NoNamedZero)10;

			var actual = serializer.Deserialize<NoNamedZero>(json);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void DeserializeSchema_TypePropertyIsArray_Issue14()
		{
			var text = "{\"type\":\"object\",\"properties\":{\"home\":{\"type\":[\"object\",\"null\"],\"properties\":{\"street\":{\"type\":\"string\"}}}}}";
			var json = JsonValue.Parse(text);
			var expected = new JsonSchema()
				.Type(JsonSchemaType.Object)
				.Property("home", new JsonSchema()
					          .Type(JsonSchemaType.Object | JsonSchemaType.Null)
					          .Property("street", new JsonSchema().Type(JsonSchemaType.String)));

			var actual = new JsonSchema();
			actual.FromJson(json, new JsonSerializer());

			Assert.AreEqual(expected, actual);
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

			var json = new { A = TimeSpan.FromSeconds(30), B = TimeSpan.FromSeconds(30) };
			var actual = serializer.Serialize(json);
			// produces {"A":"00:00:30","B":{"#Ref":"94e343ba-4ffd-4402-80be-67feb8299d90"}}

			Assert.AreEqual(expected, actual);
		}
	}
}
