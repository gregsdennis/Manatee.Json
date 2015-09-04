using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class JsonValueEqualsTest
	{
		[TestMethod]
		public void Equals_SameTypeSameValue_ReturnsTrue()
		{
			var json1 = new JsonValue(42);
			var json2 = new JsonValue(42);
			Assert.IsTrue(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_SameTypeDifferentValues_ReturnsFalse()
		{
			var json1 = new JsonValue(42);
			var json2 = new JsonValue(43);
			Assert.IsFalse(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_DifferentTypeDifferentValues_ReturnsFalse()
		{
			var json1 = new JsonValue(42);
			var json2 = new JsonValue("42");
			Assert.IsFalse(json1.Equals(json2));
		}
		[TestMethod]
		public void Equals_StringValue_ReturnsTrue()
		{
			var json = new JsonValue("42");
			var str = "42";
			Assert.IsTrue(json.Equals(str));
		}
		[TestMethod]
		public void Equals_StringValue_ReturnsFalse()
		{
			var json = new JsonValue("42");
			var str = "43";
			Assert.IsFalse(json.Equals(str));
		}
		[TestMethod]
		public void Equals_BooleanValue_ReturnsTrue()
		{
			var json = new JsonValue(true);
			var boolean = true;
			Assert.IsTrue(json.Equals(boolean));
		}
		[TestMethod]
		public void Equals_BooleanValue_ReturnsFalse()
		{
			var json = new JsonValue(true);
			var boolean = false;
			Assert.IsFalse(json.Equals(boolean));
		}
		[TestMethod]
		public void Equals_NumberValue_ReturnsTrue()
		{
			var json = new JsonValue(42);
			var number = 42;
			Assert.IsTrue(json.Equals(number));
		}
		[TestMethod]
		public void Equals_NumberValue_ReturnsFalse()
		{
			var json = new JsonValue(42);
			var number = 43;
			Assert.IsFalse(json.Equals(number));
		}
	}
}