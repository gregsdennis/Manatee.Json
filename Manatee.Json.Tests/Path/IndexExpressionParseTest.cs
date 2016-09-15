using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class IndexExpressionParseTest
	{
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
		public void Exponent()
		{
			var text = "$[(@.length^1)]";
			var expected = JsonPathWith.Array(jv => jv.Length() ^ 1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
	}
}
