using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	[Ignore]
	public class DevTest
	{
		[TestMethod]
		public void Test1()
		{
			var text = "{\"key\":4,int:\"no\"}";
			var json = JsonValue.Parse(text);
		}
	}
}