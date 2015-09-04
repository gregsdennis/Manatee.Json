using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class JsonValueAccessorTest
	{
		[TestMethod]
		public void Accessor_GetBoolWhenBool_ReturnsValue()
		{
			var json = new JsonValue(true);
			var expected = true;
			var actual = json.Boolean;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Accessor_GetNumberWhenNumber_ReturnsValue()
		{
			var json = new JsonValue(42);
			double expected = 42;
			var actual = json.Number;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Accessor_GetStringWhenString_ReturnsValue()
		{
			var json = new JsonValue("a string");
			var expected = "a string";
			var actual = json.String;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Accessor_GetArrayWhenArray_ReturnsValue()
		{
			var json = new JsonValue(new JsonArray {false, 42, "a string"});
			var expected = new JsonArray {false, 42, "a string"};
			var actual = json.Array;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Accessor_GetObjectWhenObject_ReturnsValue()
		{
			var json = new JsonValue(new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}});
			var expected = new JsonObject {{"bool", false}, {"int", 42}, {"string", "a string"}};
			var actual = json.Object;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		[ExpectedException(typeof (JsonValueIncorrectTypeException))]
		public void Accessor_GetBoolWhenNotBool_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.Boolean;
		}
		[TestMethod]
		[ExpectedException(typeof (JsonValueIncorrectTypeException))]
		public void Accessor_GetNumberWhenNotNumber_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.Number;
		}
		[TestMethod]
		[ExpectedException(typeof (JsonValueIncorrectTypeException))]
		public void Accessor_GetStringWhenNotString_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.String;
		}
		[TestMethod]
		[ExpectedException(typeof (JsonValueIncorrectTypeException))]
		public void Accessor_GetArrayWhenNotArray_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.Array;
		}
		[TestMethod]
		[ExpectedException(typeof (JsonValueIncorrectTypeException))]
		public void Accessor_GetObjectWhenNotObject_ThrowsJsonValueIncorrectTypeException()
		{
			var json = JsonValue.Null;
			var actual = json.Object;
		}
		[TestMethod]
		public void Ctor_TypeIsNull()
		{
			var json = new JsonValue();
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[TestMethod]
		public void Ctor_NullBoolean_TypeIsNull()
		{
			var json = new JsonValue((bool?) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[TestMethod]
		public void Ctor_NullDouble_TypeIsNull()
		{
			var json = new JsonValue((double?) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[TestMethod]
		public void Ctor_NullString_TypeIsNull()
		{
			var json = new JsonValue((string) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[TestMethod]
		public void Ctor_NullArray_TypeIsNull()
		{
			var json = new JsonValue((JsonArray) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
		[TestMethod]
		public void Ctor_NullObject_TypeIsNull()
		{
			var json = new JsonValue((JsonObject) null);
			Assert.AreEqual(JsonValueType.Null, json.Type);
		}
	}
}