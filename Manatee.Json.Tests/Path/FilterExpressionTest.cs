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
		[Ignore] // interestingly, it never worked like this...
		public void KeyEqualsValue()
		{
			var text = "$[?(@.test == 5)]";
			var expected = JsonPathWith.Array(jv => jv.Name("test") == 5);

			var actual = JsonPath.Parse(text);

			// this failes because the parsed version uses a raw PathExpression
			// whereas the constructed version uses a NameExpression (which contains a path)
			// TODO: Remove PathExpression derivatives.

			Assert.AreEqual(expected, actual);
		}

		[TestMethod] // this is the same test as above, just that we're validating the
					 // path evaluation rather than comparing the paths.
		public void KeyEqualsValue_Evaluation()
		{
			var text = "$[?(@.test == 5)]";
			var json = new JsonArray
				{
					new JsonObject {{"test", 5}, {"yep", 6}},
					new JsonObject {{"test", 6}, {"nope", 6}},
				};
			var path = JsonPathWith.Array(jv => jv.Name("test") == 5);
			var expected = path.Evaluate(json);
			var parsed = JsonPath.Parse(text);

			var actual = parsed.Evaluate(json);

			Assert.AreEqual(expected, actual);
		}
	}
}
