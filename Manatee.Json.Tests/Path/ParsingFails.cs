using System;
using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class ParsingFails
	{
		private static void Run(string text)
		{
			try
			{
				Console.WriteLine($"\n{JsonPath.Parse(text)}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void EmptyIndexedArray()
		{
			Run("$[]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void EmptyIndexExpression()
		{
			Run("$[()]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void EmptyFilterExpressionArray()
		{
			Run("$[?()]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void EmptyObject()
		{
			Run("$.");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void InvalidPropertyName()
		{
			Run("$.tes*t");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void StringIndex()
		{
			// TODO: This may actually be the key to parsing bracket notation
			Run("$[\"test\"]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void BoolIndex()
		{
			Run("$[false]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingCloseBracket()
		{
			Run("$[1.test");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void BadSliceFormat()
		{
			Run("$[1-5]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingCloseParenthesisOnIndexExpression()
		{
			Run("$[(1]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingCloseBracketOnIndexExpression()
		{
			Run("$[(1)");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingCloseBracketOnIndexExpression2()
		{
			Run("$[(1).test");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingCloseParenthesisOnFilterExpression()
		{
			Run("$[?(@.name == 4].test");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingCloseBracketOnFilterExpression()
		{
			Run("$[?(@.name == 4)");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingCloseBracketOnFilterExpression2()
		{
			Run("$[?(@.name == 4).test");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingDot()
		{
			Run("$name");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void DotBeforeArray()
		{
			Run("$.[0]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void TooManyDots()
		{
			Run("$...name");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void ArrayMultiIndex()
		{
			Run("$[?(@[1,3] == 5)]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void ArraySliceEqualsValue()
		{
			Run("$[?(@[1:3] == 5)]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void ArrayIndexExpressionEqualsValue()
		{
			Run("$[?(@[(@.length-1))]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void ArrayFilterExpressionEqualsValue()
		{
			Run("$[?(@[?(@.name == 5))]");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingSearchParameter()
		{
			Run("$..");
		}
		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void MissingKey()
		{
			Run("$..");
		}
	}
}
