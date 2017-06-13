﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class JsonObjectTest
	{
		[TestMethod]
		public void Equals_SameValuesSameOrder_ReturnsTrue()
		{
			var json1 = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var json2 = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			Assert.IsTrue(json1.Equals(json2));
			Assert.AreEqual(json1.GetHashCode(), json2.GetHashCode());
		}
		[TestMethod]
		public void Equals_SameValuesDifferentOrder_ReturnsTrue()
		{
			var json1 = new JsonObject {{"int", 42}, {"bool", false}, {"string", "a string"}};
			var json2 = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			Assert.IsTrue(json1.Equals(json2));
			Assert.AreEqual(json1.GetHashCode(), json2.GetHashCode());
		}
		[TestMethod]
		public void Equals_DifferentValues_ReturnsFalse()
		{
			var json1 = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var json2 = new JsonObject {{"bool", false}, {"int", "42"}, {"string", "a string"}};
			Assert.IsFalse(json1.Equals(json2));
			Assert.AreNotEqual(((JsonValue) 42).GetHashCode(), ((JsonValue) "42").GetHashCode());
			//Assert.AreNotEqual(json1.GetHashCode(), json2.GetHashCode());
		}
		[TestMethod]
		public void Parse_ValidString_ReturnsCorrectObject()
		{
			var s = "{\"bool\":false,\"int\":42,\"string\":\"a string\"}";
			var expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_EmptyObject_ReturnsEmptyObject()
		{
			var s = "{}";
			var expected = new JsonObject();
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringMissingValue_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,\"int\":,\"string\":\"a string\"}";
			try
			{
				var actual = JsonValue.Parse(s);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Cannot determine type. Path: '$.int'", e.Message);
			}
		}
		[TestMethod]
		public void Parse_StringMissingKey_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,:42,\"string\":\"a string\"}";
			try
			{
				var actual = JsonValue.Parse(s);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Expected key. Path: '$.bool'", e.Message);
			}
		}
		[TestMethod]
		public void Parse_StringMissingKeyValue_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,,\"string\":\"a string\"}";
			try
			{
				var actual = JsonValue.Parse(s);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Expected key. Path: '$.bool'", e.Message);
			}
		}
		[TestMethod]
		public void Parse_StringMissingKeyValueDelimiter_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,\"int\"42,\"string\":\"a string\"}";
			try
			{
				var actual = JsonValue.Parse(s);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Expected ':'. Path: '$.int'", e.Message);
			}
		}
		[TestMethod]
		public void Parse_StringMissingDelimiter_ThrowsJsonValueParseException()
		{
			var s = "{\"bool\":false,\"int\":42\"string\":\"a string\"}";
			try
			{
				var actual = JsonValue.Parse(s);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Expected ',', ']', or '}'. Path: '$.int'", e.Message);
			}
		}
		[TestMethod]
		public void Parse_StringMissingOpenBrace_ParsesFirstElementOnly()
		{
			var s = "\"bool\":false,\"int\":42,\"string\":\"a string\"}";
			var actual = JsonValue.Parse(s);
			Assert.AreEqual("bool", actual);
		}
		[TestMethod]
		public void Parse_StringMissingCloseBrace_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,\"int\":42,\"string\":\"a string\"";
			try
			{
				var actual = JsonValue.Parse(s);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Unexpected end of input. Path: '$.string'", e.Message);
			}
		}
		[TestMethod]
		public void Add_NullValueAddsJsonNull()
		{
			var obj = new JsonObject();
			obj.Add("null", null);

			Assert.AreEqual(1, obj.Count);
			Assert.AreEqual(JsonValue.Null, obj["null"]);
		}
		[TestMethod]
		public void Indexer_NullValueAddsJsonNull()
		{
			var obj = new JsonObject();
			obj["null"] = null;

			Assert.AreEqual(1, obj.Count);
			Assert.AreEqual(JsonValue.Null, obj["null"]);
		}
		[TestMethod]
		public void Ctor_InitializationIsSuccessful()
		{
			var expected = new JsonObject
				{
					{"number", 5},
					{"string", "a string"},
					{"bool", true},
					{"null", JsonValue.Null}
				};
			var actual = new JsonObject(expected.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
			Assert.AreEqual(expected, actual);
		}
	}
}
