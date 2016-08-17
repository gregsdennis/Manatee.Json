/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		LinqExtensionsTest.cs
	Namespace:		Manatee.Json.Tests
	Class Name:		LinqExtensionsTest
	Purpose:		Tests for the LINQ extension methods for the Json library.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;
using Manatee.Json.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class LinqExtensionsTest
	{
		[TestMethod]
		public void ToJson_JsonValueCollection_ReturnsArray()
		{
			var json = new List<JsonValue> {false, 42, "a string", "another string"};
			var expected = new JsonArray {false, 42, "a string", "another string"};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
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
		[TestMethod]
		public void ToJson_StringArray_ReturnsArray()
		{
			var json = new List<string> {"a string", "another string"};
			var expected = new JsonArray {"a string", "another string"};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullStringArray_ReturnsNull()
		{
			var json = (IEnumerable<string>) null;
			var expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_BooleanArray_ReturnsArray()
		{
			var json = new List<bool> {false, true};
			var expected = new JsonArray {false, true};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullBooleanArray_ReturnsNull()
		{
			var json = (IEnumerable<bool>) null;
			var expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullableBooleanArray_ReturnsArray()
		{
			var json = new List<bool?> {false, true, null};
			var expected = new JsonArray {false, true, JsonValue.Null};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullNullableBooleanArray_ReturnsNull()
		{
			var json = (IEnumerable<bool?>) null;
			var expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NumberArray_ReturnsArray()
		{
			var json = new List<double> {5, 6};
			var expected = new JsonArray {5, 6};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullNumberArray_ReturnsNull()
		{
			var json = (IEnumerable<double>) null;
			var expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullableNumberArray_ReturnsArray()
		{
			var json = new List<double?> {5, 6, null};
			var expected = new JsonArray {5, 6, JsonValue.Null};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullNullableNumberArray_ReturnsNull()
		{
			var json = (IEnumerable<double?>) null;
			var expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_JsonArrayArray_ReturnsArray()
		{
			var json = new List<JsonArray> {new JsonArray {false, true}, new JsonArray {"a string", "another string"}};
			var expected = new JsonArray {new JsonArray {false, true}, new JsonArray {"a string", "another string"}};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullJsonArrayArray_ReturnsNull()
		{
			var json = (IEnumerable<JsonArray>) null;
			var expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_JsonObjectArray_ReturnsArray()
		{
			var json = new List<JsonObject> {new JsonObject {{"bool", false}, {"int", 42}}, new JsonObject {{"string", "a string"}, {"string2", "another string"}}};
			var expected = new JsonArray {new JsonObject {{"bool", false}, {"int", 42}}, new JsonObject {{"string", "a string"}, {"string2", "another string"}}};
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullJsonObjectArray_ReturnsNull()
		{
			var json = (IEnumerable<JsonObject>) null;
			var expected = (JsonArray) null;
			var actual = json.ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_SerializableArray_ReturnsArray()
		{
			var serializer = new JsonSerializer();
			var json = new List<JsonSerializableClass> {new JsonSerializableClass("this", 0), new JsonSerializableClass("that", 1)};
			var expected = new JsonArray {new JsonObject {{"StringProp", "this"}, {"IntProp", 0}}, new JsonObject {{"StringProp", "that"}, {"IntProp", 1}}};
			var actual = json.ToJson(serializer);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_NullSerializableArray_ReturnsNull()
		{
			var serializer = new JsonSerializer();
			var json = (IEnumerable<JsonSerializableClass>)null;
			var expected = (JsonArray)null;
			var actual = json.ToJson(serializer);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void FromJson_SerializableArray_ReturnsArray()
		{
			var serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			var json = new JsonArray { new JsonObject { { "StringProp", "this" }, { "IntProp", 0 } }, new JsonObject { { "StringProp", "that" }, { "IntProp", 1 } } };
			var expected = new List<JsonSerializableClass> { new JsonSerializableClass("this", 0), new JsonSerializableClass("that", 1) };
			var actual = json.FromJson<JsonSerializableClass>(serializer);
			Assert.IsTrue(expected.SequenceEqual(actual));
		}
		[TestMethod]
		public void FromJson_NullSerializableArray_ReturnsNull()
		{
			var serializer = new JsonSerializer();
			var json = (JsonArray)null;
			var expected = (IEnumerable<JsonSerializableClass>)null;
			var actual = json.FromJson<JsonSerializableClass>(serializer);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void FromJson_SerializableObject_ReturnsObject()
		{
			var serializer = new JsonSerializer();
			var json = new JsonObject { { "StringProp", "this" }, { "IntProp", 0 } };
			var expected = new JsonSerializableClass("this", 0);
			var actual = json.FromJson<JsonSerializableClass>(serializer);
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
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
