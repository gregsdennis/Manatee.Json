using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Json
{
	[TestClass]
	public class LinqExtensionsTest
	{
		[TestMethod]
		public void ToJson_FilteringArray_ReturnsArray()
		{
			var json = new JsonArray {false, 42, "a string", "another string"};
			var expected = new JsonArray {"a string", "another string"};
			var actual = json.Where(jv => jv.Type == JsonValueType.String).ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_FilteringObject_ReturnsObject()
		{
			var json = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}, {"string2", "another string"}};
			var expected = new JsonObject {{"string", "a string"}, {"string2", "another string"}};
			var actual = json.Where(jv => jv.Value.Type == JsonValueType.String).ToJson();
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ToJson_FilteringObjectExtractValues_ReturnsArray()
		{
			var json = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}, {"string2", "another string"}};
			var expected = new JsonArray {"a string", "another string"};
			var actual = json.Where(jv => jv.Value.Type == JsonValueType.String)
							 .Select(jv => jv.Value).ToJson();
			Assert.AreEqual(expected, actual);
		}
	}
}
