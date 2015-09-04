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
 
	File Name:		JsonArrayExtensionsTest.cs
	Namespace:		Manatee.Json.Tests
	Class Name:		JsonArrayExtensionsTest
	Purpose:		Tests for the JsonArray extension methods for the Json library.

***************************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class JsonArrayExtensionsTest
	{
		[TestMethod]
		public void OfType_ReturnsOnlyRequestType()
		{
			var json = new JsonArray { 6, "string", false, 42, JsonValue.Null, true };
			var expected = new JsonArray { 6, 42 };
			var values = json.OfType(JsonValueType.Number);

			Assert.AreEqual(expected, values);
		}
		[TestMethod]
		public void OfType_NullSourceReturnsNull()
		{
			var json = (JsonArray)null;
			var expected = (JsonArray)null;
			var values = json.OfType(JsonValueType.Number);

			Assert.AreEqual(expected, values);
		}
	}
}