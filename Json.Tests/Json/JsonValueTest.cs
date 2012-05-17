/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonValueTest.cs
	Namespace:		Manatee.Tests.Json
	Class Name:		JsonValueTest
	Purpose:		Tests for JsonValue.

***************************************************************************************/
using Manatee.Json;
using Manatee.Json.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Manatee.Json.Exceptions;

namespace Manatee.Tests.Json
{
	[TestClass]
	public class JsonValueTest
	{
		#region Accessor Tests
		[TestMethod]
		public void Accessor_GetBoolWhenBool_ReturnsValue()
		{
			var json = new JsonValue(true);
			var expected = true;
			var actual = json.Boolean;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Accessor_GetNumberWhenNumber_ReturnsValue()
		{
			var json = new JsonValue(42);
			double expected = 42;
			var actual = json.Number;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Accessor_GetStringWhenString_ReturnsValue()
		{
			var json = new JsonValue("a string");
			var expected = "a string";
			var actual = json.String;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Accessor_GetArrayWhenArray_ReturnsValue()
		{
			var json = new JsonValue(new JsonArray{false, 42, "a string"});
			var expected = new JsonArray {false, 42, "a string"};
			var actual = json.Array;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Accessor_GetObjectWhenObject_ReturnsValue()
		{
			var json = new JsonValue(new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}});
			var expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var actual = json.Object;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonValueIncorrectTypeException))]
		public void Accessor_GetBoolWhenNotBool_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.Boolean;
		}
		[TestMethod]
		[ExpectedException(typeof(JsonValueIncorrectTypeException))]
		public void Accessor_GetNumberWhenNotNumber_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.Number;
		}
		[TestMethod]
		[ExpectedException(typeof(JsonValueIncorrectTypeException))]
		public void Accessor_GetStringWhenNotString_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.String;
		}
		[TestMethod]
		[ExpectedException(typeof(JsonValueIncorrectTypeException))]
		public void Accessor_GetArrayWhenNotArray_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.Array;
		}
		[TestMethod]
		[ExpectedException(typeof(JsonValueIncorrectTypeException))]
		public void Accessor_GetObjectWhenNotObject_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.Object;
		}
		#endregion

		#region ToString Tests
		[TestMethod]
		public void ToString_BoolFalse_ReturnsCorrectString()
		{
			var json = new JsonValue(false);
			var expected = "false";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToString_BoolTrue_ReturnsCorrectString()
		{
			var json = new JsonValue(true);
			var expected = "true";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToString_Number_ReturnsCorrectString()
		{
			var json = new JsonValue(42);
			var expected = "42";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToString_String_ReturnsCorrectString()
		{
			var json = new JsonValue("a string");
			var expected = "\"a string\"";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToString_Array_ReturnsCorrectString()
		{
			var json = new JsonValue(new JsonArray { false, 42, "a string" });
			var expected = "[false,42,\"a string\"]";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToString_Object_ReturnsCorrectString()
		{
			var json = new JsonValue(new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}});
			var expected = "{\"bool\":false,\"int\":42,\"string\":\"a string\"}";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToString_Null_ReturnsCorrectString()
		{
			var json = new JsonValue();
			var expected = "null";
			var actual = json.ToString();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToString_EscapableCharacters_ReturnsCorrectString()
		{
			JsonValue json1 = new JsonValue("These\" are/ some\\ of\b the\f escapable\n characters."),
					  json2 = new JsonValue("Here\r are\t some" + (char)0x25A0 + " more.");
			var expected1 = "\"These\\\" are\\/ some\\\\ of\\b the\\f escapable\\n characters.\"";
			var expected2 = "\"Here\\r are\\t some\\u25A0 more.\"";
			var actual1 = json1.ToString();
			var actual2 = json2.ToString();

			Assert.AreEqual(expected1, actual1);
			Assert.AreEqual(expected2, actual2);
		}
		#endregion

		#region Equals Tests
		[TestMethod]
		public void Equals_SameTypeSameValue_ReturnsTrue()
		{
			var json1 = new JsonValue(42);
			var json2 = new JsonValue(42);
			Assert.IsTrue(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_SameTypeDifferentValues_ReturnsFalse()
		{
			var json1 = new JsonValue(42);
			var json2 = new JsonValue(43);
			Assert.IsFalse(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_DifferentTypeDifferentValues_ReturnsFalse()
		{
			var json1 = new JsonValue(42);
			var json2 = new JsonValue("42");
			Assert.IsFalse(json1.Equals(json2));
		}
		#endregion

		#region Parse Tests
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
			var expected = new JsonValue(new JsonArray {false, 42, "a string"});
			var actual = JsonValue.Parse(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_StringWithObject_ReturnsCorrectJsonValue()
		{
			var s = "{\"bool\":false,\"int\":42,\"string\":\"a string\"}";
			var expected = new JsonValue(new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}});
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
		[ExpectedException(typeof(JsonValueParseException))]
		public void Parse_StringWithBadToken_ThrowsJsonValueParseException()
		{
			var s = "invalid data";
			var actual = JsonValue.Parse(s);
		}
		[TestMethod]
		public void Parse_StringValueWithEscapedQuote_ReturnsCorrectJsonValue()
		{
			var json = "\"An \\\"escaped\\\" quote\"";
			JsonValue expected = "An \"escaped\" quote";
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
			var json = "\"An \\/escaped\\/ reverse solidus\"";
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
		#endregion

		#region Operator Tests
		[TestMethod]
		public void CastOperator_Bool_AssignsCorrectValue()
		{
			var json = new JsonValue();
			Assert.AreEqual(JsonValueType.Null, json.Type);
			json = false;
			var expected = false;
			Assert.AreEqual(JsonValueType.Boolean, json.Type);
			Assert.AreEqual(expected, json.Boolean);
		}
		[TestMethod]
		public void CastOperator_Number_AssignsCorrectValue()
		{
			var json = new JsonValue();
			Assert.AreEqual(JsonValueType.Null, json.Type);
			json = 42;
			var expected = 42;
			Assert.AreEqual(JsonValueType.Number, json.Type);
			Assert.AreEqual(expected, json.Number);
		}
		[TestMethod]
		public void CastOperator_String_AssignsCorrectValue()
		{
			var json = new JsonValue();
			Assert.AreEqual(JsonValueType.Null, json.Type);
			json = "a string";
			var expected = "a string";
			Assert.AreEqual(JsonValueType.String, json.Type);
			Assert.AreEqual(expected, json.String);
		}
		[TestMethod]
		public void CastOperator_Array_AssignsCorrectValue()
		{
			var json = new JsonValue();
			Assert.AreEqual(JsonValueType.Null, json.Type);
			json = new JsonArray {false, 42, "a string", "another string"};
			var expected = new JsonArray {false, 42, "a string", "another string"};
			Assert.AreEqual(JsonValueType.Array, json.Type);
			Assert.AreEqual(expected, json.Array);
		}
		[TestMethod]
		public void CastOperator_Object_AssignsCorrectValue()
		{
			var json = new JsonValue();
			Assert.AreEqual(JsonValueType.Null, json.Type);
			json = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			Assert.AreEqual(JsonValueType.Object, json.Type);
			Assert.AreEqual(expected, json.Object);
		}
		#endregion
	}
}
