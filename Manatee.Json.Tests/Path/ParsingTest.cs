using Manatee.Json.Path;
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

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleQuotedNamedObject()
		{
			var text = "$.'quoted name'";
			var expected = JsonPathWith.Name("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DoubleQuotedNamedObject()
		{
			var text = "$.\"quoted name\"";
			var expected = JsonPathWith.Name("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleWildcardObject()
		{
			var text = "$.*";
			var expected = JsonPathWith.Wildcard();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void NamedObjectWithWildcardObject()
		{
			var text = "$.name.*";
			var expected = JsonPathWith.Name("name").Wildcard();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void WildcardObjectWithNamedObject()
		{
			var text = "$.*.name";
			var expected = JsonPathWith.Wildcard().Name("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void SingleNamedSearch()
		{
			var text = "$..name";
			var expected = JsonPathWith.Search("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleQuotedNamedSearch()
		{
			var text = "$..'quoted name'";
			var expected = JsonPathWith.Search("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DoubleQuotedNamedSearch()
		{
			var text = "$..\"quoted name\"";
			var expected = JsonPathWith.Search("quoted name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SingleWildcardSearch()
		{
			var text = "$..*";
			var expected = JsonPathWith.Search();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void NamedObjectWithWildcardSearch()
		{
			var text = "$.name..*";
			var expected = JsonPathWith.Name("name").Search();

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void WildcardObjectWithNamedSearch()
		{
			var text = "$.*..name";
			var expected = JsonPathWith.Wildcard().Search("name");

			var actual = JsonPath.Parse(text);

			Assert.AreEqual(expected.ToString(), actual.ToString());
			//Assert.AreEqual(expected, actual);
		}
	}
}
