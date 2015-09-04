using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class JsonValueParseTest
	{
		[TestMethod]
		public void Parse_StringWithBoolFalse_ReturnsCorrectJsonValue()
		{
			var s = "false";
			var expected = new JsonValue(false);
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringWithBoolTrue_ReturnsCorrectJsonValue()
		{
			var s = "true";
			var expected = new JsonValue(true);
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringWithNumber_ReturnsCorrectJsonValue()
		{
			var s = "42";
			var expected = new JsonValue(42);
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringWithString_ReturnsCorrectJsonValue()
		{
			var s = "\"a string\"";
			var expected = new JsonValue("a string");
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringWithArray_ReturnsCorrectJsonValue()
		{
			var s = "[false,42,\"a string\"]";
			var expected = new JsonValue(new JsonArray { false, 42, "a string" });
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringWithObject_ReturnsCorrectJsonValue()
		{
			var s = "{\"bool\":false,\"int\":42,\"string\":\"a string\"}";
			var expected = new JsonValue(new JsonObject { { "bool", false }, { "int", 42 }, { "string", "a string" } });
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringWithNull_ReturnsCorrectJsonValue()
		{
			var s = "null";
			var expected = JsonValue.Null;
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringWithNulf_ReturnsCorrectJsonValue()
		{
			var s = "nulf";
			try
			{
				var actual = JsonValue.Parse(s);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Value not recognized: 'nulf'. Path: '$'", e.Message);
			}
		}
		[TestMethod]
		public void Parse_StringWithBadToken_ThrowsJsonValueParseException()
		{
			var s = "invalid data";
			try
			{
				var actual = JsonValue.Parse(s);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Cannot determine type. Path: '$'", e.Message);
			}
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedQuote_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\\"escaped quote with\\\" spaces\"";
			JsonValue expected = "An \"escaped quote with\" spaces";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedSolidus_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\\\escaped\\\\ solidus\"";
			JsonValue expected = "An \\escaped\\ solidus";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedReverseSolidus_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\/escaped/ reverse solidus\"";
			JsonValue expected = "An /escaped/ reverse solidus";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedBackspace_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\bescaped\\b backspace\"";
			JsonValue expected = "An \bescaped\b backspace";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedFormFeed_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\fescaped\\f form feed\"";
			JsonValue expected = "An \fescaped\f form feed";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedNewLine_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\nescaped\\n new line\"";
			JsonValue expected = "An \nescaped\n new line";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedCarriageReturn_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\rescaped\\r carriage return\"";
			JsonValue expected = "An \rescaped\r carriage return";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedHorizontalTab_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\tescaped\\t horizontal tab\"";
			JsonValue expected = "An \tescaped\t horizontal tab";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedHexadecimalValue_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\u25A0escaped\\u25A0 hex value\"";
			JsonValue expected = "An " + (char)0x25A0 + "escaped" + (char)0x25A0 + " hex value";
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithSurrogateUnicodePair_ReturnsCorrectJsonValue()
		{
			var json = "\"\\uD85A\\uDC21\"";
			JsonValue expected = char.ConvertFromUtf32(0x16821);
			var actual = JsonValue.Parse(json);

			Assert.AreEqual(expected.String, actual.String);
		}
		[TestMethod]
		public void Parse_StringValueWithInvalidEscapeSequence_ThrowsException()
		{
			var json = "\"An \\rescaped\\a carriage return\"";
			try
			{
				var actual = JsonValue.Parse(json);
			}
			catch (JsonSyntaxException e)
			{
				Assert.AreEqual("Invalid escape sequence: '\\a'. Path: '$'", e.Message);
			}
		}
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Parse_NullString_ThrowsException()
		{
			string json = null;
			var actual = JsonValue.Parse(json);
		}
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Parse_EmptyString_ThrowsException()
		{
			string json = string.Empty;
			var actual = JsonValue.Parse(json);
		}
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Parse_WhitespaceString_ThrowsException()
		{
			string json = "  \t\n";
			var actual = JsonValue.Parse(json);
		}
		[TestMethod]
		public void Parse_Escaping()
		{
			var str = "{\"string\":\"double\\n\\nspaced\"}";
			var json = JsonValue.Parse(str).Object;
			Console.WriteLine(json["string"].String);
		}
		[TestMethod]
		[DeploymentItem("TrelloCard.json")]
		public void Parse_TrelloCard()
		{
			var str = File.ReadAllText("TrelloCard.json");
			var json = JsonValue.Parse(str);
			Console.WriteLine(json);
		}
	}
}