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
	public class FilterExpressionTest
	{
		[TestMethod]
		public void KeyEqualsValue()
		{
			var text = "$[?(@.test == 5)]";
			var expected = JsonPathWith.Array(jv => jv.Name("test").Name("second") == 5);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
	}
}
