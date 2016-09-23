using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class IndexExpressionParseTest
	{
		private void Run(JsonPath expected, string text)
		{
			var actual = JsonPath.Parse(text);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Length()
		{
			var text = "$[(@.length)]";
			var expected = JsonPathWith.Array(jv => jv.Length());

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Length_Root()
		{
			var text = "$[($.length)]";
			var expected = JsonPathWith.Array(jv => JsonPathRoot.Length());

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void ExtendedLength()
		{
			var text = "$[(@.name.length)]";
			var expected = JsonPathWith.Array(jv => jv.Name("name").Length());

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Addition()
		{
			var text = "$[(@.length+1)]";
			var expected = JsonPathWith.Array(jv => jv.Length() + 1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Subtraction()
		{
			var text = "$[(@.length-1)]";
			var expected = JsonPathWith.Array(jv => jv.Length() - 1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Multiplication()
		{
			var text = "$[(@.length*1)]";
			var expected = JsonPathWith.Array(jv => jv.Length()*1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Division()
		{
			var text = "$[(@.length/1)]";
			var expected = JsonPathWith.Array(jv => jv.Length()/1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Modulus()
		{
			var text = "$[(@.length%1)]";
			var expected = JsonPathWith.Array(jv => jv.Length()%1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		[Ignore]
		// The C# ^ operator doesn't have the required exponentiation priority, so
		// constructing expressions with this operator results in a strange structure.
		// Also not really sure JS supports it as an exponentiation operator, either.
		public void Exponent()
		{
			var text = "$[(@.length^1)]";
			var expected = JsonPathWith.Array(jv => jv.Length() ^ 1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Add3()
		{
			var text = "$[(3+@.length+3)]";
			var expected = JsonPathWith.Array(jv => 3 + jv.Length() + 3);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void WhyOnGodsGreenEarthWouldAnyoneDoThis()
		{
			var text = "$[(4+@.length*($.name.length-2)+5)]";
			var expected = JsonPathWith.Array(jv => 4 + jv.Length()*(JsonPathRoot.Name("name").Length() - 2) + 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void JustAnInteger()
		{
			Run(JsonPathWith.Array(jv => 1), "$[(1)]");
		}
	}
}
