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