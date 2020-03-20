
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonValueToStringTest
	{
		[Test]
		public void ToString_BoolFalse_ReturnsCorrectString()
		{
			var json = new JsonValue(false);
			var expected = "false";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_BoolTrue_ReturnsCorrectString()
		{
			var json = new JsonValue(true);
			var expected = "true";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_Number_ReturnsCorrectString()
		{
			var json = new JsonValue(42);
			var expected = "42";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_String_ReturnsCorrectString()
		{
			var json = new JsonValue("a string");
			var expected = "\"a string\"";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_Array_ReturnsCorrectString()
		{
			var json = new JsonValue(new JsonArray { false, 42, "a string" });
			var expected = "[false,42,\"a string\"]";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_EmptyArray_ReturnsCorrectString()
		{
			var json = new JsonValue(new JsonArray());
			var expected = "[]";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_Object_ReturnsCorrectString()
		{
			var json = new JsonValue(new JsonObject { { "bool", false }, { "int", 42 }, { "string", "a string" } });
			var expected = "{\"bool\":false,\"int\":42,\"string\":\"a string\"}";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_EmptyObject_ReturnsCorrectString()
		{
			var json = new JsonValue(new JsonObject());
			var expected = "{}";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_Null_ReturnsCorrectString()
		{
			var json = JsonValue.Null;
			var expected = "null";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ToString_EscapableCharacters_ReturnsCorrectString()
		{
			JsonValue json1 = new JsonValue("These\" are/ some\\ of\b the\f escapable\n characters."),
					  json2 = new JsonValue("Here\r are\t some" + (char)0x25A0 + " more" + (char)0x009F + ".");
			var expected1 = "\"These\\\" are/ some\\\\ of\\b the\\f escapable\\n characters.\"";
			var expected2 = "\"Here\\r are\\t some" + (char)0x25A0 + " more\\u009F.\"";
			var actual1 = json1.ToString();
			var actual2 = json2.ToString();

			Assert.AreEqual(expected1, actual1);
			Assert.AreEqual(expected2, actual2);
		}
		[Test]
		public void GetIndentedString_Boolean_ReturnsCorrectIndention()
		{
			var json = new JsonValue(false);
			var expected = "false";
			var actual = json.GetIndentedString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GetIndentedString_Number_ReturnsCorrectIndention()
		{
			var json = new JsonValue(42);
			var expected = "42";
			var actual = json.GetIndentedString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GetIndentedString_String_ReturnsCorrectIndention()
		{
			var json = new JsonValue("a string");
			var expected = "\"a string\"";
			var actual = json.GetIndentedString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GetIndentedString_Null_ReturnsCorrectIndention()
		{
			var json = JsonValue.Null;
			var expected = "null";
			var actual = json.GetIndentedString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GetIndentedString_Array_ReturnsCorrectIndention()
		{
			var json = new JsonValue(new JsonArray { false, 42, "a string" });
			var expected = "[\n\tfalse,\n\t42,\n\t\"a string\"\n]";
			var actual = json.GetIndentedString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GetIndentedString_EmptyArray_ReturnsCorrectIndention()
		{
			var json = new JsonValue(new JsonArray());
			var expected = "[]";
			var actual = json.GetIndentedString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GetIndentedString_Object_ReturnsCorrectIndention()
		{
			var json = new JsonValue(new JsonObject { { "bool", false }, { "int", 42 }, { "string", "a string" } });
			var expected = "{\n\t\"bool\" : false,\n\t\"int\" : 42,\n\t\"string\" : \"a string\"\n}";
			var actual = json.GetIndentedString();
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void GetIndentedString_EmptyObject_ReturnsCorrectIndention()
		{
			var json = new JsonValue(new JsonObject());
			var expected = "{}";
			var actual = json.GetIndentedString();
			Assert.AreEqual(expected, actual);
		}
	}
}
