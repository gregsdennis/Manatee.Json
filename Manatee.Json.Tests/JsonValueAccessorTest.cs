
using System;
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
		public void Ctor_NullString_TypeIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonValue((string) null));
		}
		[Test]
		public void Ctor_NullArray_TypeIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonValue((JsonArray) null));
		}
		[Test]
		public void Ctor_NullObject_TypeIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonValue((JsonObject) null));
		}
	}
}