using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class FilterExpressionParseTest
	{
		private static void Run(JsonPath expected, string text)
		{
			var actual = JsonPath.Parse(text);
			Assert.AreEqual(expected, actual);
		}

		private static void CompareEval(JsonPath expected, string text)
		{
			var data = new JsonArray
				{
					1,
					new JsonObject
						{
							["bool"] = false,
							["int"] = 20,
							["string"] = "value",
						},
					"hello",
					true,
					5,
					new JsonArray {1,2,3 }
				};

			var actual = JsonPath.Parse(text);

			var expectedResult = expected.Evaluate(data);
			var actualResult = actual.Evaluate(data);

			Assert.AreEqual(expectedResult, actualResult);
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
		// This won't work the same.  Parsing generates an IndexOfExpression with a distinct parameter,
		// but when constructing the path, the parameter goes through several castings generating a
		// parameter expression. The parsed path would be different in structure but should still
		// represent the same thing.  We have to test by evaluation.
		public void IndexOfNumber()
		{
			CompareEval(JsonPathWith.Array(jv => jv.IndexOf(5) == 4), "$[?(@.indexOf(5) == 4)]");
		}
		[TestMethod]
		// This won't work the same.  Parsing generates an IndexOfExpression with a distinct parameter,
		// but when constructing the path, the parameter goes through several castings generating a
		// parameter expression. The parsed path would be different in structure but should still
		// represent the same thing.  We have to test by evaluation.
		public void IndexOfBoolean()
		{
			CompareEval(JsonPathWith.Array(jv => jv.IndexOf(true) == 3), "$[?(@.indexOf(true) == 3)]");
		}
		[TestMethod]
		// This won't work the same.  Parsing generates an IndexOfExpression with a distinct parameter,
		// but when constructing the path, the parameter goes through several castings generating a
		// parameter expression. The parsed path would be different in structure but should still
		// represent the same thing.  We have to test by evaluation.
		public void IndexOfString()
		{
			CompareEval(JsonPathWith.Array(jv => jv.IndexOf("string") == 2), "$[?(@.indexOf(\"hello\") == 2)]");
		}
		[TestMethod]
		// This won't work the same.  Parsing generates a ValueExpression, but the only way to
		// construct the path is to pass the field, which generates a FieldExpression. The parsed
		// path would be different in structure but should still represent the same thing.
		// We have to test by evaluation.
		public void IndexOfArray()
		{
			var arr = new JsonArray {1, 2, 3};
			CompareEval(JsonPathWith.Array(jv => jv.IndexOf(arr) == 6), "$[?(@.indexOf([1,2,3]) == 6)]");
		}
		[TestMethod]
		// This won't work the same.  Parsing generates a ValueExpression, but the only way to
		// construct the path is to pass the field, which generates a FieldExpression. The parsed
		// path would be different in structure but should still represent the same thing.
		// We have to test by evaluation.
		public void IndexOfObject()
		{
			var obj = new JsonObject
				{
					["bool"] = false,
					["int"] = 20,
					["string"] = "value",
				};
			CompareEval(JsonPathWith.Array(jv => jv.IndexOf(obj) == 1), "$[?(@.indexOf({\"key\":\"value\"}) == 1)]");
		}
	}
}
