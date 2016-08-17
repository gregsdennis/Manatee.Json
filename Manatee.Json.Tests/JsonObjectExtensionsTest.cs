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
 
	File Name:		JsonObjectExtensionsTest.cs
	Namespace:		Manatee.Json.Tests
	Class Name:		JsonObjectExtensionsTest
	Purpose:		Tests for the JsonObject extension methods for the Json library.

***************************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class JsonObjectExtensionsTest
	{
		[TestMethod]
		public void TryGetString_ReturnsRequested()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"boolean", true}
				};

			var value = json.TryGetString("string");

			Assert.AreEqual("test", value);
		}
		[TestMethod]
		public void TryGetString_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetString("string");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetString_ValueNotFoundReturnsNull()
		{
			var json = new JsonObject
				{
					{"number", 6},
					{"boolean", true}
				};
			var value = json.TryGetString("string");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetString_ValueNotStringReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"boolean", true}
				};
			var value = json.TryGetString("number");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetNumber_ReturnsRequested()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"boolean", true}
				};

			var value = json.TryGetNumber("number");

			Assert.AreEqual(6.0, value);
		}
		[TestMethod]
		public void TryGetNumber_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetNumber("number");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetNumber_ValueNotFoundReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"boolean", true}
				};
			var value = json.TryGetNumber("number");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetNumber_ValueNotNumberReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"boolean", true}
				};
			var value = json.TryGetNumber("string");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetBoolean_ReturnsRequested()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"boolean", true}
				};

			var value = json.TryGetBoolean("boolean");

			Assert.AreEqual(true, value);
		}
		[TestMethod]
		public void TryGetBoolean_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetBoolean("boolean");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetBoolean_ValueNotFoundReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6}
				};
			var value = json.TryGetBoolean("boolean");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetBoolean_ValueNotBooleanReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"boolean", true}
				};
			var value = json.TryGetBoolean("string");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetArray_ReturnsRequested()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"array", new JsonArray()}
				};

			var value = json.TryGetArray("array");

			Assert.AreEqual(new JsonArray(), value);
		}
		[TestMethod]
		public void TryGetArray_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetArray("array");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetArray_ValueNotFoundReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6}
				};
			var value = json.TryGetArray("array");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetArray_ValueNotBooleanReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"array", new JsonArray()}
				};
			var value = json.TryGetArray("string");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetObject_ReturnsRequested()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"object", new JsonObject()}
				};

			var value = json.TryGetObject("object");

			Assert.AreEqual(new JsonObject(), value);
		}
		[TestMethod]
		public void TryGetObject_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetObject("object");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetObject_ValueNotFoundReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6}
				};
			var value = json.TryGetObject("object");

			Assert.IsNull(value);
		}
		[TestMethod]
		public void TryGetObject_ValueNotBooleanReturnsNull()
		{
			var json = new JsonObject
				{
					{"string", "test"},
					{"number", 6},
					{"object", new JsonObject()}
				};
			var value = json.TryGetObject("string");

			Assert.IsNull(value);
		}
	}
}
