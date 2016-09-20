using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class FilterExpressionTest
	{
		private void Run(JsonPath expected, string text)
		{
			var actual = JsonPath.Parse(text);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void PropertyEqualsValue()
		{
			Run(JsonPathWith.Array(jv => jv.Name("test") == 5), "$[?(@.test == 5)]");
		}
		[TestMethod]
		public void PropertyNotEqualToValue()
		{
			var text = "$[?(@.test != 5)]";
			var expected = JsonPathWith.Array(jv => jv.Name("test") != 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void PropertyGreaterThanValue()
		{
			var text = "$[?(@.test > 5)]";
			var expected = JsonPathWith.Array(jv => jv.Name("test") > 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void PropertyGreaterThanEqualToValue()
		{
			var text = "$[?(@.test >= 5)]";
			var expected = JsonPathWith.Array(jv => jv.Name("test") >= 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void PropertyLessThanValue()
		{
			var text = "$[?(@.test < 5)]";
			var expected = JsonPathWith.Array(jv => jv.Name("test") < 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void PropertyLessThanEqualToValue()
		{
			var text = "$[?(@.test <= 5)]";
			var expected = JsonPathWith.Array(jv => jv.Name("test") <= 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ArrayIndexEqualsValue()
		{
			var text = "$[?(@[1] == 5)]";
			var expected = JsonPathWith.Array(jv => jv.ArrayIndex(1) == 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void ArrayMultiIndex()
		{
			var text = "$[?(@[1,3] == 5)]";

			try
			{
				var actual = JsonPath.Parse(text);
			}
			catch (JsonPathSyntaxException e)
			{
				Assert.IsTrue(e.Message.StartsWith("JSON Path expression indexers only support single constant values."));
				throw;
			}
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void ArraySliceEqualsValue()
		{
			var text = "$[?(@[1:3] == 5)]";

			try
			{
				var actual = JsonPath.Parse(text);
			}
			catch (JsonPathSyntaxException e)
			{
				Assert.IsTrue(e.Message.StartsWith("JSON Path expression indexers only support single constant values."));
				throw;
			}
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void ArrayIndexExpressionEqualsValue()
		{
			var text = "$[?(@[(@.length-1))]";

			try
			{
				var actual = JsonPath.Parse(text);
			}
			catch (JsonPathSyntaxException e)
			{
				Assert.IsTrue(e.Message.StartsWith("JSON Path expression indexers only support single constant values."));
				throw;
			}
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void ArrayFilterExpressionEqualsValue()
		{
			var text = "$[?(@[?(@.name == 5))]";

			try
			{
				var actual = JsonPath.Parse(text);
			}
			catch (JsonPathSyntaxException e)
			{
				Assert.IsTrue(e.Message.StartsWith("JSON Path expression indexers only support single constant values."));
				throw;
			}
		}
		[TestMethod]
		public void HasProperty()
		{
			var text = "$[?(@.test)]";
			var expected = JsonPathWith.Array(jv => jv.HasProperty("test"));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void DoesNotHaveProperty()
		{
			var text = "$[?(!@.test)]";
			var expected = JsonPathWith.Array(jv => !jv.HasProperty("test"));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void GroupedNot()
		{
			var text = "$[?(!(@.test && @.name == 5))]";
			var expected = JsonPathWith.Array(jv => !(jv.HasProperty("test") && jv.Name("name") == 5));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void And()
		{
			var text = "$[?(@.test && @.name == 5)]";
			var expected = JsonPathWith.Array(jv => jv.HasProperty("test") && jv.Name("name") == 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Or()
		{
			var text = "$[?(@.test || @.name == 5)]";
			var expected = JsonPathWith.Array(jv => jv.HasProperty("test") || jv.Name("name") == 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void IndexOfNumber()
		{
			Run(JsonPathWith.Array(jv => jv.IndexOf(5) == 4), "$[?(@.indexOf(5) == 4)]");
		}
		[TestMethod]
		public void IndexOfBoolean()
		{
			Run(JsonPathWith.Array(jv => jv.IndexOf(false) == 4), "$[?(@.indexOf(false) == 4)]");
		}
		[TestMethod]
		public void IndexOfString()
		{
			Run(JsonPathWith.Array(jv => jv.IndexOf("string") == 4), "$[?(@.indexOf(\"string\") == 4)]");
		}
		[TestMethod]
		[Ignore]
		// This won't work because there's not a way to get a FieldExpression when parsing,
		// and there's not a way to construct this with a ValueExpression as a JsonArray.
		// TODO: This should still create a valid path.
		public void IndexOfArray()
		{
			var arr = new JsonArray {1, 2, 3};
			Run(JsonPathWith.Array(jv => jv.IndexOf(arr) == 4), "$[?(@.indexOf([1,2,3]) == 4)]");
		}
		[TestMethod]
		[Ignore]
		// This won't work because there's not a way to get a FieldExpression when parsing,
		// and there's not a way to construct this with a ValueExpression as a JsonObject.
		// TODO: This should still create a valid path.
		public void IndexOfObject()
		{
			var obj = new JsonObject {{"key", "value"}};
			Run(JsonPathWith.Array(jv => jv.IndexOf(obj) == 4), "$[?(@.indexOf({\"key\":\"value\"}) == 4)]");
		}
	}
}
