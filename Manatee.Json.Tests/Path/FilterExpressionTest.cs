using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class FilterExpressionTest
	{
		[TestMethod]
		public void PropertyEqualsValue()
		{
			var text = "$[?(@.test == 5)]";
			var expected = JsonPathWith.Array(jv => jv.Name("test") == 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
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
	}
}
