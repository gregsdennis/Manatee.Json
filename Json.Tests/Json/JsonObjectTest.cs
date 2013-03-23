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
 
	File Name:		JsonObjectTest.cs
	Namespace:		Manatee.Tests.Json
	Class Name:		JsonObjectTest
	Purpose:		Tests for JsonObject.

***************************************************************************************/
using Manatee.Json;
using Manatee.Json.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Tests.Json
{
	[TestClass]
	public class JsonObjectTest
	{
		[TestMethod]
		public void ToString_ReturnsCorrectString()
		{
			var json = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var expected = "{\"bool\":false,\"int\":42,\"string\":\"a string\"}";
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
			var s = "{\"bool\":false,\"int\":42,\"string\":\"a string\"}";
			var expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var actual = new JsonObject(s);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Parse_EmptyObject_ReturnsEmptyObject()
		{
			var s = "{}";
			var expected = new JsonObject();
			var i = 0;
			var actual = new JsonObject(s, ref i);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingValue_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,\"int\":,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingKey_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,:42,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingKeyValue_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingKeyValueDelimiter_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,\"int\"42,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonValueParseException))]
		public void Parse_StringMissingDelimiter_ThrowsJsonValueParseException()
		{
			var s = "{\"bool\":false,\"int\":42\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingOpenBrace_ThrowsJsonSyntaxException()
		{
			var s = "\"bool\":false,\"int\":42,\"string\":\"a string\"}";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonSyntaxException))]
		public void Parse_StringMissingCloseBrace_ThrowsJsonSyntaxException()
		{
			var s = "{\"bool\":false,\"int\":42,\"string\":\"a string\"";
			var actual = new JsonObject(s);
		}
		[TestMethod]
		public void Parse_StringFromSourceForge()
		{
			var s = @"{
  ""self"": ""self"",
  ""name"": ""name"",
  ""emailAddress"": ""test at test dot com"",
  ""avatarUrls"": {
	""16x16"": ""http://smallUrl"",
	""48x48"": ""https://largeUrl""
  },
  ""displayName"": ""Display Name"",
  ""active"": true,
  ""timeZone"": ""Europe"",
  ""groups"": {
	""size"": 1,
	""items"": [
	  {
		""name"": ""users""
	  }
	]
  },
  ""expand"": ""groups""
}";
			var actual = new JsonObject(s);
			var newString = actual.ToString();
		}
	}
}
