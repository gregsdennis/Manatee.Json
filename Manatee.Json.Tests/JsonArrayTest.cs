
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonArrayTest
	{
		[Test]
		public void Equals_SameValuesSameOrder_ReturnsTrue()
		{
			var json1 = new JsonArray { false, 42, "a string" };
			var json2 = new JsonArray { false, 42, "a string" };
			Assert.IsTrue(json1.Equals(json2));
		}
		[Test]
		public void Equals_SameValuesDifferentOrder_ReturnsFalse()
		{
			var json1 = new JsonArray { false, 42, "a string" };
			var json2 = new JsonArray { 42, false, "a string" };
			Assert.IsFalse(json1.Equals(json2));
		}
		[Test]
		public void Equals_DifferentValues_ReturnsFalse()
		{
			var json1 = new JsonArray { false, 42, "a string" };
			var json2 = new JsonArray { false, "42", "a string" };
			Assert.IsFalse(json1.Equals(json2));
		}
		[Test]
		public void Parse_ValidString_ReturnsCorrectArray()
		{
			var s = "[false,42,\"a string\"]";
			JsonValue expected = new JsonArray { false, 42, "a string" };
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Parse_EmptyArray_ReturnsEmptyArray()
		{
			var s = "[]";
			JsonValue expected = new JsonArray();
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Parse_StringMissingValue_ThrowsJsonSyntaxException()
		{
			Assert.Throws<JsonSyntaxException>(() =>
				{
					var s = "[false,,\"a string\"]";
					var actual = JsonValue.Parse(s);
				});
		}
		[Test]
		public void Parse_StringMissingOpenBracket_ReturnsFirstElement()
		{
			var s = "false,42,\"a string\"]";
			var actual = JsonValue.Parse(s);
			Assert.AreEqual((JsonValue) false, actual);
		}
		[Test]
		public void Parse_StringMissingCloseBracket_ThrowsJsonSyntaxException()
		{
			Assert.Throws<JsonSyntaxException>(() =>
				{
					var s = "[false,42,\"a string\"";
					var actual = JsonValue.Parse(s);
				});
		}
		[Test]
		public void Parse_IncompleteEndingWithWhitespaceBetweenTokens_ThrowsJsonSyntaxException()
		{
			Assert.Throws<JsonSyntaxException>(() =>
				{
					var s = "[  false,  42,  \"a string\"  ";
					var actual = JsonValue.Parse(s);
				});
		}
		[Test]
		public void Ctor_InitializationIsSuccessful()
		{
			var expected = new JsonArray {5, "a string", false, JsonValue.Null};
			var actual = new JsonArray(expected.ToArray());
			Assert.AreEqual(expected, actual);
		}
	}
}
