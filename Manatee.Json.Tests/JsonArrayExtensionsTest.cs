
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonArrayExtensionsTest
	{
		[Test]
		public void OfType_ReturnsOnlyRequestType()
		{
			var json = new JsonArray { 6, "string", false, 42, JsonValue.Null, true };
			var expected = new JsonArray { 6, 42 };
			var values = json.OfType(JsonValueType.Number);

			Assert.AreEqual(expected, values);
		}
		[Test]
		public void OfType_NullSourceReturnsNull()
		{
			var json = (JsonArray)null;
			var expected = (JsonArray)null;
			var values = json.OfType(JsonValueType.Number);

			Assert.AreEqual(expected, values);
		}
	}
}