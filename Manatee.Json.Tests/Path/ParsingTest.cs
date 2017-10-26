using Manatee.Json.Path;
using NUnit.Framework;

namespace Manatee.Json.Tests.Path
{
	[TestFixture]
	public class ParsingTest
	{
		private static void Run(JsonPath expected, string text)
		{
			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void SingleNamedObject()
		{
			Run(JsonPathWith.Name("name"), "$.name");
		}
		[Test]
		public void SingleQuotedNamedObject()
		{
			Run(JsonPathWith.Name("quoted name"), "$.'quoted name'");
		}
		[Test]
		public void DoubleQuotedNamedObject()
		{
			Run(JsonPathWith.Name("quoted name"), "$.\"quoted name\"");
		}
		[Test]
		public void SingleWildcardObject()
		{
			Run(JsonPathWith.Name(), "$.*");
		}
		[Test]
		public void NamedObjectWithWildcardObject()
		{
			var text = "$.name.*";
			var expected = JsonPathWith.Name("name").Name();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void WildcardObjectWithNamedObject()
		{
			var text = "$.*.name";
			var expected = JsonPathWith.Name().Name("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SingleNamedSearch()
		{
			var text = "$..name";
			var expected = JsonPathWith.Search("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SingleQuotedNamedSearch()
		{
			var text = "$..'quoted name'";
			var expected = JsonPathWith.Search("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void DoubleQuotedNamedSearch()
		{
			var text = "$..\"quoted name\"";
			var expected = JsonPathWith.Search("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SingleWildcardSearch()
		{
			var text = "$..*";
			var expected = JsonPathWith.Search();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void NamedObjectWithWildcardSearch()
		{
			var text = "$.name..*";
			var expected = JsonPathWith.Name("name").Search();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void WildcardObjectWithNamedSearch()
		{
			var text = "$.*..name";
			var expected = JsonPathWith.Name().Search("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SingleIndexedArray()
		{
			var text = "$[1]";
			var expected = JsonPathWith.Array(1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SingleSlicedArray()
		{
			var text = "$[1:5]";
			var expected = JsonPathWith.Array(new Slice(1, 5));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SteppedSlicedArray()
		{
			var text = "$[1:5:2]";
			var expected = JsonPathWith.Array(new Slice(1, 5, 2));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void IndexedSlicedArray()
		{
			var text = "$[1,5:7]";
			var expected = JsonPathWith.Array(1, new Slice(5, 7));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SlicedIndexedArray()
		{
			var text = "$[1:5,7]";
			var expected = JsonPathWith.Array(new Slice(1, 5), 7);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void MultiSlicedArray()
		{
			var text = "$[1:5,7:11:2]";
			var expected = JsonPathWith.Array(new Slice(1, 5), new Slice(7, 11, 2));

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void MultiIndexedArray()
		{
			var text = "$[1,3]";
			var expected = JsonPathWith.Array(1, 3);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void WildcardArray()
		{
			var text = "$[*]";
			var expected = JsonPathWith.Array();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void SearchIndexedArray()
		{
			var text = "$..[1]";
			var expected = JsonPathWith.SearchArray(1);

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void ChainedNameIndexedArray()
		{
			Run(JsonPathWith.Name("name").Array(4), "$.name[4]");
		}
		[Test]
		public void ChainedIndexedArrayName()
		{
			Run(JsonPathWith.Array(4).Name("name"), "$[4].name");
		}
		[Test]
		public void ChainedNameName()
		{
			Run(JsonPathWith.Name("name").Name("test"), "$.name.test");
		}
		[Test]
		public void ChainedIndexedArrayIndexedArray()
		{
			Run(JsonPathWith.Array(2).Array(4), "$[2][4]");
		}
		[Test]
		public void MultipleConditionsAdd()
		{
			Run(JsonPathWith.Array(jv => jv.Length() == 3 && jv.ArrayIndex(1) == false), "$[?(@.length == 3 && @[1] == false)]");
		}
		[Test]
		public void MultipleConditionsOr()
		{
			Run(JsonPathWith.Array(jv => jv.Length() == 3 || jv.ArrayIndex(1) == false), "$[?(@.length == 3 || @[1] == false)]");
		}
		[Test]
		public void Group()
		{
			Run(JsonPathWith.Array(jv => (jv.Length()+1)*2 == 6), "$[?((@.length+1)*2 == 6)]");
		}
		[Test]
		public void NotGroup()
		{
			// ReSharper disable once NegativeEqualityExpression
			// Don't simplify this.  It's a parsing test.
			Run(JsonPathWith.Array(jv => !(jv.Length() == 3) && jv.ArrayIndex(1) == false), "$[?(!(@.length == 3) && @[1] == false)]");
		}
		[Test]
		public void WeirdPropertyNameQuoted()
		{
			Run(JsonPathWith.Name("tes* \"t"), "$.\"tes* \\\"t\"");
		}
		[Test]
		public void EmptyKey()
		{
			Run(JsonPathWith.Name(""), "$.''");
		}
		[Test]
		public void EmptySearch()
		{
			Run(JsonPathWith.Search(""), "$..''");
		}
	}
}
