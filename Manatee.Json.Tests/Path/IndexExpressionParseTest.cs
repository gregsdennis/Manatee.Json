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
	}
}
