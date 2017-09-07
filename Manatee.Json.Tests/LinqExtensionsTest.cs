using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;
// ReSharper disable ExpressionIsAlwaysNull

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class LinqExtensionsTest
	{
		[Test]
		public void ToJson_JsonValueCollection_ReturnsArray()
		{
			var json = new List<JsonValue> {false, 42, "a string", "another string"};
			var expected = new JsonArray {false, 42, "a string", "another string"};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_KeyValuePairCollection_ReturnsObject()
		{
			var json = new List<KeyValuePair<string, JsonValue>>
				{
					new KeyValuePair<string, JsonValue>("bool", false),
					new KeyValuePair<string, JsonValue>("int", 42),
					new KeyValuePair<string, JsonValue>("string", "a string"),
					new KeyValuePair<string, JsonValue>("string2", "another string")
				};
			var expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}, {"string2", "another string"}};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_StringArray_ReturnsArray()
		{
			var json = new List<string> {"a string", "another string"};
			JsonValue expected = new JsonArray {"a string", "another string"};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullStringArray_ReturnsNull()
		{
			var json = (IEnumerable<string>) null;
			JsonValue expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_BooleanArray_ReturnsArray()
		{
			var json = new List<bool> {false, true};
			JsonValue expected = new JsonArray {false, true};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullBooleanArray_ReturnsNull()
		{
			var json = (IEnumerable<bool>) null;
			JsonValue expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullableBooleanArray_ReturnsArray()
		{
			var json = new List<bool?> {false, true, null};
			JsonValue expected = new JsonArray {false, true, JsonValue.Null};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullNullableBooleanArray_ReturnsNull()
		{
			var json = (IEnumerable<bool?>) null;
			JsonValue expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NumberArray_ReturnsArray()
		{
			var json = new List<double> {5, 6};
			JsonValue expected = new JsonArray {5, 6};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullNumberArray_ReturnsNull()
		{
			var json = (IEnumerable<double>) null;
			JsonValue expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullableNumberArray_ReturnsArray()
		{
			var json = new List<double?> {5, 6, null};
			JsonValue expected = new JsonArray {5, 6, JsonValue.Null};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullNullableNumberArray_ReturnsNull()
		{
			var json = (IEnumerable<double?>) null;
			JsonValue expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_JsonArrayArray_ReturnsArray()
		{
			var json = new List<JsonArray> {new JsonArray {false, true}, new JsonArray {"a string", "another string"}};
			JsonValue expected = new JsonArray {new JsonArray {false, true}, new JsonArray {"a string", "another string"}};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullJsonArrayArray_ReturnsNull()
		{
			var json = (IEnumerable<JsonArray>) null;
			JsonValue expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_JsonObjectArray_ReturnsArray()
		{
			var json = new List<JsonObject> {new JsonObject {{"bool", false}, {"int", 42}}, new JsonObject {{"string", "a string"}, {"string2", "another string"}}};
			JsonValue expected = new JsonArray {new JsonObject {{"bool", false}, {"int", 42}}, new JsonObject {{"string", "a string"}, {"string2", "another string"}}};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullJsonObjectArray_ReturnsNull()
		{
			var json = (IEnumerable<JsonObject>) null;
			JsonValue expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_SerializableArray_ReturnsArray()
		{
			var serializer = new JsonSerializer();
			var json = new List<JsonSerializableClass> {new JsonSerializableClass("this", 0), new JsonSerializableClass("that", 1)};
			JsonValue expected = new JsonArray {new JsonObject {{"StringProp", "this"}, {"IntProp", 0}}, new JsonObject {{"StringProp", "that"}, {"IntProp", 1}}};
			var actual = json.ToJson(serializer);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToJson_NullSerializableArray_ReturnsNull()
		{
			var serializer = new JsonSerializer();
			var json = (IEnumerable<JsonSerializableClass>)null;
			JsonValue expected = (JsonArray)null;
			var actual = json.ToJson(serializer);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void FromJson_SerializableArray_ReturnsArray()
		{
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			var json = new JsonArray { new JsonObject { { "StringProp", "this" }, { "IntProp", 0 } }, new JsonObject { { "StringProp", "that" }, { "IntProp", 1 } } };
			var expected = new List<JsonSerializableClass> { new JsonSerializableClass("this", 0), new JsonSerializableClass("that", 1) };
			var actual = json.FromJson<JsonSerializableClass>(serializer);
			Assert.IsTrue(expected.SequenceEqual(actual));
		}
		[Test]
		public void FromJson_NullSerializableArray_ReturnsNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				{
					var serializer = new JsonSerializer();
					var json = (JsonArray) null;
					json.FromJson<JsonSerializableClass>(serializer).ToList();
				});
		}
		[Test]
		public void FromJson_SerializableObject_ReturnsObject()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject { { "StringProp", "this" }, { "IntProp", 0 } };
			var expected = new JsonSerializableClass("this", 0);
			var actual = json.FromJson<JsonSerializableClass>(serializer);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void FromJson_NullSerializableObject_ReturnsNull()
		{
			var serializer = new JsonSerializer();
			var json = (JsonObject)null;
			var expected = (IEnumerable<JsonSerializableClass>)null;
			var actual = json.FromJson<JsonSerializableClass>(serializer);
			Assert.AreEqual(expected, actual);
		}
	}
}
