using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using Manatee.Json.Internal;
using Manatee.Json.Patch;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Schema.TestSuite;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	// TODO: Add categories to exclude this test.
	//[Ignore("This test fixture for development purposes only.")]
	public class DevTest
	{
		[Test]
		public void Test1()
		{
			JsonSerializationTypeRegistry.RegisterType(_SerializeDynamic, _DeserializeDynamic);
			JsonSerializationTypeRegistry.RegisterType(_SerializeExpando, _DeserializeExpando);

			dynamic dyn = new ExpandoObject();
			dyn.StringProp = "string";
			dyn.IntProp = 5;
			dyn.NestProp = new ExpandoObject();
			dyn.NestProp.Value = false;

			JsonValue expected = new JsonObject
				{
					["StringProp"] = "string",
					["IntProp"] = 5,
					["NestProp"] = new JsonObject
						{
							["Value"] = false
						}
				};


			var serializer = new JsonSerializer();
			var json = serializer.Serialize<dynamic>(dyn);

			Assert.AreEqual(expected, json);
		}

		[Test]
		public void InverseTest1()
		{
			JsonSerializationTypeRegistry.RegisterType(_SerializeDynamic, _DeserializeDynamic);

			var json = new JsonObject
				{
					["StringProp"] = "string",
					["IntProp"] = 5,
					["NestProp"] = new JsonObject
						{
							["Value"] = false
						}
				};
			var serializer = new JsonSerializer();

			var dyn = serializer.Deserialize<dynamic>(json);

			Assert.AreEqual("string", dyn.StringProp);
			Assert.AreEqual(5, dyn.IntProp);
			Assert.AreEqual(false, dyn.NestProp.Value);
		}

		private static JsonValue _SerializeDynamic(dynamic input, JsonSerializer serializer)
		{
			if (input is ExpandoObject expando)
			{
				var dict = (IDictionary<string, object>) expando;
				return dict.ToDictionary(kvp => kvp.Key, kvp => serializer.Serialize<dynamic>(kvp.Value))
				           .ToJson();
			}
			var type = (Type) input.GetType();
			var serializerCopy = new JsonSerializer {Options = serializer.Options};
			serializerCopy.Options.EncodeDefaultValues = true;
			var serializeMethod = typeof(JsonSerializer).GetTypeInfo()
			                                            .DeclaredMethods
			                                            .Single(m => m.Name == nameof(JsonSerializer.Serialize) &&
			                                                         !m.IsStatic)
			                                            .MakeGenericMethod(type);
			return (JsonValue) serializeMethod.Invoke(serializerCopy, new object[] {input});
		}

		private static dynamic _DeserializeDynamic(JsonValue json, JsonSerializer serializer)
		{
			switch (json.Type)
			{
				case JsonValueType.Number:
					return json.Number;
				case JsonValueType.String:
					return json.String;
				case JsonValueType.Boolean:
					return json.Boolean;
				case JsonValueType.Array:
					return json.Array.Select(jv => _DeserializeDynamic(jv, serializer)).ToList();
				case JsonValueType.Object:
					var result = new ExpandoObject() as IDictionary<string, object>;
					foreach (var kvp in json.Object)
					{
						result[kvp.Key] = _DeserializeDynamic(kvp.Value, serializer);
					}
					return result;
				case JsonValueType.Null:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static JsonValue _SerializeExpando(ExpandoObject input, JsonSerializer serializer)
		{
			if (input is ExpandoObject expando)
			{
				var dict = (IDictionary<string, object>) expando;
				return dict.ToDictionary(kvp => kvp.Key, kvp => serializer.Serialize<dynamic>(kvp.Value))
				           .ToJson();
			}
			var type = (Type) input.GetType();
			var serializerCopy = new JsonSerializer {Options = serializer.Options};
			serializerCopy.Options.EncodeDefaultValues = true;
			var serializeMethod = typeof(JsonSerializer).GetTypeInfo()
			                                            .DeclaredMethods
			                                            .Single(m => m.Name == nameof(JsonSerializer.Serialize) &&
			                                                         !m.IsStatic)
			                                            .MakeGenericMethod(type);
			return (JsonValue) serializeMethod.Invoke(serializerCopy, new object[] {input});
		}

		private static ExpandoObject _DeserializeExpando(JsonValue json, JsonSerializer serializer)
		{
			var result = new ExpandoObject() as IDictionary<string, object>;
			foreach (var kvp in json.Object)
			{
				result[kvp.Key] = _DeserializeDynamic(kvp.Value, serializer);
			}
			return (ExpandoObject) result;
		}
	}
}