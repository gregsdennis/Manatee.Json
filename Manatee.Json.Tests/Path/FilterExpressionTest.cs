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
			Run(JsonPathWith.Array(jv => jv.Name("test") != 5), "$[?(@.test != 5)]");
		}
		[TestMethod]
		public void PropertyGreaterThanValue()
		{
			Run(JsonPathWith.Array(jv => jv.Name("test") > 5), "$[?(@.test > 5)]");
		}
		[TestMethod]
		public void PropertyGreaterThanEqualToValue()
		{
			Run(JsonPathWith.Array(jv => jv.Name("test") >= 5), "$[?(@.test >= 5)]");
		}
		[TestMethod]
		public void PropertyLessThanValue()
		{
			Run(JsonPathWith.Array(jv => jv.Name("test") < 5), "$[?(@.test < 5)]");
		}
		[TestMethod]
		public void PropertyLessThanEqualToValue()
		{
			Run(JsonPathWith.Array(jv => jv.Name("test") <= 5), "$[?(@.test <= 5)]");
		}
		[TestMethod]
		public void ArrayIndexEqualsValue()
		{
			Run(JsonPathWith.Array(jv => jv.ArrayIndex(1) == 5), "$[?(@[1] == 5)]");
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
			Run(JsonPathWith.Array(jv => jv.HasProperty("test")), "$[?(@.test)]");
		}
		[TestMethod]
		public void DoesNotHaveProperty()
		{
			Run(JsonPathWith.Array(jv => !jv.HasProperty("test")), "$[?(!@.test)]");
		}
		[TestMethod]
		public void GroupedNot()
		{
			Run(JsonPathWith.Array(jv => !(jv.HasProperty("test") && jv.Name("name") == 5)), "$[?(!(@.test && @.name == 5))]");
		}
		[TestMethod]
		public void And()
		{
			Run(JsonPathWith.Array(jv => jv.HasProperty("test") && jv.Name("name") == 5), "$[?(@.test && @.name == 5)]");
		}
		[TestMethod]
		public void Or()
		{
			Run(JsonPathWith.Array(jv => jv.HasProperty("test") || jv.Name("name") == 5),"$[?(@.test || @.name == 5)]");
		}
		[TestMethod]
		[Ignore]
		// This won't work the same.  Parsing generates an IndexOfExpression with a distinct parameter,
		// but when constructing the path, the parameter goes through several castings generating a
		// parameter expression. The parsed path would be different in structure but should still
		// represent the same thing.  We have to 
		// TODO: Test by evaluation
		public void IndexOfNumber()
		{
			Run(JsonPathWith.Array(jv => jv.IndexOf(5) == 4), "$[?(@.indexOf(5) == 4)]");
		}
		[TestMethod]
		[Ignore]
		// This won't work the same.  Parsing generates an IndexOfExpression with a distinct parameter,
		// but when constructing the path, the parameter goes through several castings generating a
		// parameter expression. The parsed path would be different in structure but should still
		// represent the same thing.
		// TODO: Test by evaluation
		public void IndexOfBoolean()
		{
			Run(JsonPathWith.Array(jv => jv.IndexOf(false) == 4), "$[?(@.indexOf(false) == 4)]");
		}
		[TestMethod]
		[Ignore]
		// This won't work the same.  Parsing generates an IndexOfExpression with a distinct parameter,
		// but when constructing the path, the parameter goes through several castings generating a
		// parameter expression. The parsed path would be different in structure but should still
		// represent the same thing.
		// TODO: Test by evaluation
		public void IndexOfString()
		{
			Run(JsonPathWith.Array(jv => jv.IndexOf("string") == 4), "$[?(@.indexOf(\"string\") == 4)]");
		}
		[TestMethod]
		[Ignore]
		// This won't work the same.  Parsing generates a ValueExpression, but the only way to
		// construct the path is to pass the field, which generates a FieldExpression. The parsed
		// path would be different in structure but should still represent the same thing.
		// TODO: Test by evaluation
		public void IndexOfArray()
		{
			var arr = new JsonArray {1, 2, 3};
			Run(JsonPathWith.Array(jv => jv.IndexOf(arr) == 4), "$[?(@.indexOf([1,2,3]) == 4)]");
		}
		[TestMethod]
		[Ignore]
		// This won't work the same.  Parsing generates a ValueExpression, but the only way to
		// construct the path is to pass the field, which generates a FieldExpression. The parsed
		// path would be different in structure but should still represent the same thing.
		// TODO: Test by evaluation
		public void IndexOfObject()
		{
			var obj = new JsonObject {{"key", "value"}};
			Run(JsonPathWith.Array(jv => jv.IndexOf(obj) == 4), "$[?(@.indexOf({\"key\":\"value\"}) == 4)]");
		}
	}
}
