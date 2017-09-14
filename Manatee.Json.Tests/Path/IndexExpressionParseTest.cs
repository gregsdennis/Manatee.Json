using Manatee.Json.Path;
using NUnit.Framework;

namespace Manatee.Json.Tests.Path
{
	[TestFixture]
	public class IndexExpressionParseTest
	{
		private static void _Run(JsonPath expected, string text)
		{
			var actual = JsonPath.Parse(text);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Length()
		{
			_Run(JsonPathWith.Array(jv => jv.Length()), "$[(@.length)]");
		}
		[Test]
		public void Length_Root()
		{
			_Run(JsonPathWith.Array(jv => JsonPathRoot.Length()), "$[($.length)]");
		}
		[Test]
		public void ExtendedLength()
		{
			_Run(JsonPathWith.Array(jv => jv.Name("name").Length()), "$[(@.name.length)]");
		}
		[Test]
		public void Addition()
		{
			_Run(JsonPathWith.Array(jv => jv.Length() + 1), "$[(@.length+1)]");
		}
		[Test]
		public void Subtraction()
		{
			_Run(JsonPathWith.Array(jv => jv.Length() - 1), "$[(@.length-1)]");
		}
		[Test]
		public void Multiplication()
		{
			_Run(JsonPathWith.Array(jv => jv.Length() * 1), "$[(@.length*1)]");
		}
		[Test]
		public void Division()
		{
			_Run(JsonPathWith.Array(jv => jv.Length() / 1), "$[(@.length/1)]");
		}
		[Test]
		public void Modulus()
		{
			_Run(JsonPathWith.Array(jv => jv.Length() % 1), "$[(@.length%1)]");
		}
		[Test]
		[Ignore("The ^ operator is not supported")]
		// The C# ^ operator doesn't have the required exponentiation priority, so
		// constructing expressions with this operator results in a strange structure.
		// Also not really sure JS supports it as an exponentiation operator, either.
		public void Exponent()
		{
			_Run(JsonPathWith.Array(jv => jv.Length() ^ 1), "$[(@.length^1)]");
		}
		[Test]
		public void Add3()
		{
			_Run(JsonPathWith.Array(jv => 3 + jv.Length() + 3), "$[(3+@.length+3)]");
		}
		[Test]
		public void WhyOnGodsGreenEarthWouldAnyoneDoThis()
		{
			_Run(JsonPathWith.Array(jv => 4 + jv.Length()*(JsonPathRoot.Name("name").Length() - 2) + 5),
			    "$[(4+@.length*($.name.length-2)+5)]");
		}
		[Test]
		public void JustAnInteger()
		{
			_Run(JsonPathWith.Array(jv => 1), "$[(1)]");
		}
	}
}
