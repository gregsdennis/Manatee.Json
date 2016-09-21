using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class EvaluationTest
	{
		[TestMethod]
		public void ArrayLengthExpression_LastItem()
		{
			var json = new JsonArray {1, 2, 3};
			var path = JsonPathWith.Array(jv => jv.Length() - 1);
			var expected = new JsonArray {3};

			var actual = path.Evaluate(json);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ObjectNameLengthExpression_LastItem()
		{
			var json = new JsonObject
				{
						{"name", new JsonArray {1, 2, 3}},
						{"name2", new JsonArray {1, 2, 3}},
						{"test", new JsonArray {1, 2}},
				};
			var path = JsonPathWith.Array(jv => JsonPathRoot.Name("test").Length());
			var expected = new JsonArray {new JsonArray {1, 2}};

			var actual = path.Evaluate(json);

			Assert.AreEqual(expected, actual);
		}
	}
}
