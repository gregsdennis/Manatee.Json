using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonObjectExtensionsTest
	{
		[Test]
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
		[Test]
		public void TryGetString_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetString("string");

			Assert.IsNull(value);
		}
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
		public void TryGetNumber_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetNumber("number");

			Assert.IsNull(value);
		}
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
		public void TryGetBoolean_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetBoolean("boolean");

			Assert.IsNull(value);
		}
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
		public void TryGetArray_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetArray("array");

			Assert.IsNull(value);
		}
		[Test]
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
		[Test]
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
		[Test]
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
		[Test]
		public void TryGetObject_NullObjectReturnsNull()
		{
			JsonObject json = null;

			var value = json.TryGetObject("object");

			Assert.IsNull(value);
		}
		[Test]
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
		[Test]
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
