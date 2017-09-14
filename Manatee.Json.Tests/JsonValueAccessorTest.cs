
using NUnit.Framework;
// ReSharper disable UnusedVariable

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonValueAccessorTest
	{
		[Test]
		public void Accessor_GetBoolWhenBool_ReturnsValue()
		{
			var json = new JsonValue(true);
			var actual = json.Boolean;
			Assert.AreEqual(true, actual);
		}
		[Test]
		public void Accessor_GetNumberWhenNumber_ReturnsValue()
		{
			var json = new JsonValue(42);
			double expected = 42;
			var actual = json.Number;
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Accessor_GetStringWhenString_ReturnsValue()
		{
			var json = new JsonValue("a string");
			var expected = "a string";
			var actual = json.String;
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Accessor_GetArrayWhenArray_ReturnsValue()
		{
			var json = new JsonValue(new JsonArray {false, 42, "a string"});
			var expected = new JsonArray {false, 42, "a string"};
			var actual = json.Array;
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Accessor_GetObjectWhenObject_ReturnsValue()
		{
			var json = new JsonValue(new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}});
			var expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var actual = json.Object;
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void Accessor_GetBoolWhenNotBool_ThrowsJsonValueIncorrectTypeException()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					var json = JsonValue.Null;
					var actual = json.Boolean;
				});
		}
		[Test]
		public void Accessor_GetNumberWhenNotNumber_ThrowsJsonValueIncorrectTypeException()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					var json = JsonValue.Null;
					var actual = json.Number;
				});
		}
		[Test]
		public void Accessor_GetStringWhenNotString_ThrowsJsonValueIncorrectTypeException()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					var json = JsonValue.Null;
					var actual = json.String;
				});
		}
		[Test]
		public void Accessor_GetArrayWhenNotArray_ThrowsJsonValueIncorrectTypeException()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					var json = JsonValue.Null;
					var actual = json.Array;
				});
		}
		[Test]
		public void Accessor_GetObjectWhenNotObject_ThrowsJsonValueIncorrectTypeException()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					var json = JsonValue.Null;
					var actual = json.Object;
				});
		}
		[Test]
		public void Ctor_TypeIsNull()
		{
			var json = new JsonValue();
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[Test]
		public void Ctor_NullBoolean_TypeIsNull()
		{
			var json = new JsonValue((bool?) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[Test]
		public void Ctor_NullDouble_TypeIsNull()
		{
			var json = new JsonValue((double?) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[Test]
		public void Ctor_NullString_TypeIsNull()
		{
			var json = new JsonValue((string) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[Test]
		public void Ctor_NullArray_TypeIsNull()
		{
			var json = new JsonValue((JsonArray) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[Test]
		public void Ctor_NullObject_TypeIsNull()
		{
			var json = new JsonValue((JsonObject) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
	}
}