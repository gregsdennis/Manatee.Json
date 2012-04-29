using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Json
{
	[TestClass]
	public class JsonArrayTest
	{
		[TestMethod]
		public void ToString_ReturnsCorrectString()
		{
			var json = new JsonArray {false, 42, "a string"};
			var expected = "[False,42,\"a string\"]";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Equals_SameValuesSameOrder_ReturnsTrue()
		{
			var json1 = new JsonArray { false, 42, "a string" };
			var json2 = new JsonArray { false, 42, "a string" };
			Assert.IsTrue(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_SameValuesDifferentOrder_ReturnsTrue()
		{
			var json1 = new JsonArray { false, 42, "a string" };
			var json2 = new JsonArray { 42, false, "a string" };
			Assert.IsTrue(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_DifferentValues_ReturnsFalse()
		{
			var json1 = new JsonArray { false, 42, "a string" };
			var json2 = new JsonArray { false, "42", "a string" };
			Assert.IsFalse(json1.Equals(json2));
		}
		[TestMethod]
		public void Parse_ValidString_ReturnsCorrectArray()
		{
			var s = "[False,42,\"a string\"]";
			var expected = new JsonArray { false, 42, "a string" };
			var i = 0;
			var actual = new JsonArray(s, ref i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingValue_ThrowsJsonSyntaxException()
		{
			var s = "[False,,\"a string\"]";
			var i = 0;
			var actual = new JsonArray(s, ref i);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingOpenBracket_ThrowsJsonSyntaxException()
		{
			var s = "False,42,\"a string\"]";
			var i = 0;
			var actual = new JsonArray(s, ref i);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingCloseBracket_ThrowsJsonSyntaxException()
		{
			var s = "[False,42,\"a string\"";
			var i = 0;
			var actual = new JsonArray(s, ref i);
		}
	}
}
