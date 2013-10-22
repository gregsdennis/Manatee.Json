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
 
	File Name:		LinqExtensionsTest.cs
	Namespace:		Manatee.Json.Tests.Extensions
	Class Name:		LinqExtensionsTest
	Purpose:		Tests for the LINQ extension methods for the Json library.

***************************************************************************************/

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Extensions
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
