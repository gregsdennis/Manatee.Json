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
 
	File Name:		JsonArrayTest.cs
	Namespace:		Manatee.Tests.Json
	Class Name:		JsonArrayTest
	Purpose:		Tests for JsonArray.

***************************************************************************************/
using Manatee.Json;
using Manatee.Json.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Tests.Json
{
	[TestClass]
	public class JsonArrayTest
	{
		[TestMethod]
		public void ToString_ReturnsCorrectString()
		{
			var json = new JsonArray {false, 42, "a string"};
			var expected = "[false,42,\"a string\"]";
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
			var s = "[false,42,\"a string\"]";
			var expected = new JsonArray { false, 42, "a string" };
			var i = 0;
			var actual = new JsonArray(s, ref i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_EmptyArray_ReturnsEmptyArray()
		{
			var s = "[]";
			var expected = new JsonArray();
			var i = 0;
			var actual = new JsonArray(s, ref i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingValue_ThrowsJsonSyntaxException()
		{
			var s = "[false,,\"a string\"]";
			var i = 0;
			var actual = new JsonArray(s, ref i);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingOpenBracket_ThrowsJsonSyntaxException()
		{
			var s = "false,42,\"a string\"]";
			var i = 0;
			var actual = new JsonArray(s, ref i);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingCloseBracket_ThrowsJsonSyntaxException()
		{
			var s = "[false,42,\"a string\"";
			var i = 0;
			var actual = new JsonArray(s, ref i);
		}
	}
}
