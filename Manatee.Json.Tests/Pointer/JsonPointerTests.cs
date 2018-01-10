using System;
using System.Collections;
using Manatee.Json.Pointer;
using NUnit.Framework;

namespace Manatee.Json.Tests.Pointer
{
	[TestFixture]
	public class JsonPointerTests
	{
		public static IEnumerable ErrorCases
		{
			get
			{
				yield return new TestCaseData("/d", "/d");
				yield return new TestCaseData("/a/0", "/a/0");
				yield return new TestCaseData("/d/something", "/d");
				yield return new TestCaseData("/b/false", "/b/false");
				yield return new TestCaseData("/b/-1", "/b/-1");
				yield return new TestCaseData("/b/5", "/b/5");
				yield return new TestCaseData("/c/1", "/c/1");
			}
		}

		[TestCaseSource(nameof(ErrorCases))]
		public void Errors(string pointerString, string expectedError)
		{
			var target = new JsonObject
				{
					["a"] = 1,
					["b"] = new JsonArray {5, true, null},
					["c"] = new JsonObject
						{
							["false"] = false
						}
				};


			var pointer = JsonPointer.Parse(pointerString);

			var actual = pointer.Evaluate(target);

			Assert.AreEqual($"No value found at '{expectedError}'", actual.Error);
		}

		[Test]
		public void IndexingAnObjectInterpretsIndexAsKey()
		{
			var target = new JsonObject
				{
					["a"] = 1,
					["b"] = new JsonArray {5, true, null},
					["c"] = new JsonObject
						{
							["0"] = false
						}
				};


			var pointer = JsonPointer.Parse("/c/0");

			var actual = pointer.Evaluate(target);

			if (actual.Error != null)
				Console.WriteLine(actual.Error);

			Assert.AreEqual((JsonValue) false, actual.Result);
		}

		[Test]
		public void GettingLastItemInArray()
		{
			var target = new JsonObject
				{
					["a"] = 1,
					["b"] = new JsonArray {5, true, null},
					["c"] = new JsonObject
						{
							["0"] = false
						}
				};


			var pointer = JsonPointer.Parse("/b/-");

			var actual = pointer.Evaluate(target);

			if (actual.Error != null)
				Console.WriteLine(actual.Error);

			Assert.AreEqual(JsonValue.Null, actual.Result);
		}
	}
}