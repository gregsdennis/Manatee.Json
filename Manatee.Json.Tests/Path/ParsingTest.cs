using Manatee.Json.Path;
using Manatee.Json.Path.ArrayParameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class ParsingTest
	{
		[TestMethod]
		public void SingleNamedObject()
		{
			var text = "$.name";
			var expected = JsonPathWith.Name("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleQuotedNamedObject()
		{
			var text = "$.'quoted name'";
			var expected = JsonPathWith.Name("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DoubleQuotedNamedObject()
		{
			var text = "$.\"quoted name\"";
			var expected = JsonPathWith.Name("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleWildcardObject()
		{
			var text = "$.*";
			var expected = JsonPathWith.Wildcard();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void NamedObjectWithWildcardObject()
		{
			var text = "$.name.*";
			var expected = JsonPathWith.Name("name").Wildcard();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void WildcardObjectWithNamedObject()
		{
			var text = "$.*.name";
			var expected = JsonPathWith.Wildcard().Name("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void SingleNamedSearch()
		{
			var text = "$..name";
			var expected = JsonPathWith.Search("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleQuotedNamedSearch()
		{
			var text = "$..'quoted name'";
			var expected = JsonPathWith.Search("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DoubleQuotedNamedSearch()
		{
			var text = "$..\"quoted name\"";
			var expected = JsonPathWith.Search("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleWildcardSearch()
		{
			var text = "$..*";
			var expected = JsonPathWith.Search();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void NamedObjectWithWildcardSearch()
		{
			var text = "$.name..*";
			var expected = JsonPathWith.Name("name").Search();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void WildcardObjectWithNamedSearch()
		{
			var text = "$.*..name";
			var expected = JsonPathWith.Wildcard().Search("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleIndexedArray()
		{
			var text = "$[1]";
			var expected = JsonPathWith.Array(1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleSlicedArray()
		{
			var text = "$[1:5]";
			var expected = JsonPathWith.Array(new Slice(1, 5));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SteppedSlicedArray()
		{
			var text = "$[1:5:2]";
			var expected = JsonPathWith.Array(new Slice(1, 5, 2));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void IndexedSlicedArray()
		{
			var text = "$[1,5:7]";
			var expected = JsonPathWith.Array(1, new Slice(5, 7));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SlicedIndexedArray()
		{
			var text = "$[1:5,7]";
			var expected = JsonPathWith.Array(new Slice(1, 5), 7);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MultiSlicedArray()
		{
			var text = "$[1:5,7:11:2]";
			var expected = JsonPathWith.Array(new Slice(1, 5), new Slice(7, 11, 2));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MultiIndexedArray()
		{
			var text = "$[1,3]";
			var expected = JsonPathWith.Array(1, 3);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void EmptyIndexedArray()
		{
			var text = "$[]";

			JsonPath.Parse(text);
		}

		[TestMethod]
		[ExpectedException(typeof(JsonPathSyntaxException))]
		public void EmptyObject()
		{
			var text = "$.";

			JsonPath.Parse(text);
		}

		[TestMethod]
		public void WildcardArray()
		{
			var text = "$[*]";
			var expected = JsonPathWith.Array();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SearchIndexedArray()
		{
			var text = "$..[1]";
			var expected = JsonPathWith.SearchArray(1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
	}
}
