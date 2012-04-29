using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Json
{
	[TestClass]
	public class JsonObjectTest
	{
		[TestMethod]
		public void ToString_ReturnsCorrectString()
		{
			var json = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var expected = "{\"bool\":False,\"int\":42,\"string\":\"a string\"}";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Equals_SameValuesSameOrder_ReturnsTrue()
		{
			var json1 = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var json2 = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			Assert.IsTrue(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_SameValuesDifferentOrder_ReturnsTrue()
		{
			var json1 = new JsonObject {{"int", 42}, {"bool", false}, {"string", "a string"}};
			var json2 = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			Assert.IsTrue(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_DifferentValues_ReturnsFalse()
		{
			var json1 = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var json2 = new JsonObject {{"bool", false}, {"int", "42"}, {"string", "a string"}};
			Assert.IsFalse(json1.Equals(json2));
		}
		[TestMethod]
		public void Parse_ValidString_ReturnsCorrectObject()
		{
			var s = "{\"bool\":False,\"int\":42,\"string\":\"a string\"}";
			var expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var actual = new JsonObject(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingValue_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":False,\"int\":,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingKey_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":False,:42,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingKeyValue_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":False,,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingKeyValueDelimiter_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":False,\"int\"42,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonValueParseException))]
		public void Parse_StringMissingDelimiter_ThrowsJsonValueParseException()
		{
			var s = "{\"bool\":False,\"int\":42\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingOpenBrace_ThrowsJsonSyntaxException()
		{
			var s = "\"bool\":False,\"int\":42,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingCloseBrace_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":False,\"int\":42,\"string\":\"a string\"";
			var actual = new JsonObject(s);
		}
	}
}
