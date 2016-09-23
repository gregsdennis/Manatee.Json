using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class IndexExpressionParseTest
	{
		private static void Run(JsonPath expected, string text)
		{
			var actual = JsonPath.Parse(text);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Length()
		{
			Run(JsonPathWith.Array(jv => jv.Length()), "$[(@.length)]");
		}
		[TestMethod]
		public void Length_Root()
		{
			Run(JsonPathWith.Array(jv => JsonPathRoot.Length()), "$[($.length)]");
		}
		[TestMethod]
		public void ExtendedLength()
		{
			Run(JsonPathWith.Array(jv => jv.Name("name").Length()), "$[(@.name.length)]");
		}
		[TestMethod]
		public void Addition()
		{
			Run(JsonPathWith.Array(jv => jv.Length() + 1), "$[(@.length+1)]");
		}
		[TestMethod]
		public void Subtraction()
		{
			Run(JsonPathWith.Array(jv => jv.Length() - 1), "$[(@.length-1)]");
		}
		[TestMethod]
		public void Multiplication()
		{
			Run(JsonPathWith.Array(jv => jv.Length() * 1), "$[(@.length*1)]");
		}
		[TestMethod]
		public void Division()
		{
			Run(JsonPathWith.Array(jv => jv.Length() / 1), "$[(@.length/1)]");
		}
		[TestMethod]
		public void Modulus()
		{
			Run(JsonPathWith.Array(jv => jv.Length() % 1), "$[(@.length%1)]");
		}
		[TestMethod]
		[Ignore]
		// The C# ^ operator doesn't have the required exponentiation priority, so
		// constructing expressions with this operator results in a strange structure.
		// Also not really sure JS supports it as an exponentiation operator, either.
		public void Exponent()
		{
			Run(JsonPathWith.Array(jv => jv.Length() ^ 1), "$[(@.length^1)]");
		}
		[TestMethod]
		public void Add3()
		{
			Run(JsonPathWith.Array(jv => 3 + jv.Length() + 3), "$[(3+@.length+3)]");
		}
		[TestMethod]
		public void WhyOnGodsGreenEarthWouldAnyoneDoThis()
		{
			Run(JsonPathWith.Array(jv => 4 + jv.Length()*(JsonPathRoot.Name("name").Length() - 2) + 5),
			    "$[(4+@.length*($.name.length-2)+5)]");
		}
		[TestMethod]
		public void JustAnInteger()
		{
			Run(JsonPathWith.Array(jv => 1), "$[(1)]");
		}
	}
}
