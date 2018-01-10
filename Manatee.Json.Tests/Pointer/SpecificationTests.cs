using System;
using System.Collections;
using Manatee.Json.Pointer;
using NUnit.Framework;

namespace Manatee.Json.Tests.Pointer
{
	[TestFixture]
	public class SpecificationTests
	{
		public static IEnumerable ExampleCases
		{
			get
			{
				// Basic
				yield return new TestCaseData("", (JsonValue) new JsonObject
					{
						["foo"] = new JsonArray {"bar", "baz"},
						[""] = 0,
						["a/b"] = 1,
						["c%d"] = 2,
						["e^f"] = 3,
						["g|h"] = 4,
						["i\\j"] = 5,
						["k\"l"] = 6,
						[" "] = 7,
						["m~n"] = 8,
					});
				yield return new TestCaseData("/foo", (JsonValue) new JsonArray{"bar", "baz"});
				yield return new TestCaseData("/foo/0", new JsonValue("bar"));
				yield return new TestCaseData("/", new JsonValue(0));
				yield return new TestCaseData("/a~1b", new JsonValue(1));
				yield return new TestCaseData("/c%d", new JsonValue(2));
				yield return new TestCaseData("/e^f", new JsonValue(3));
				yield return new TestCaseData("/g|h", new JsonValue(4));
				yield return new TestCaseData("/i\\j", new JsonValue(5));
				yield return new TestCaseData("/k\"l", new JsonValue(6));
				yield return new TestCaseData("/ ", new JsonValue(7));
				yield return new TestCaseData("/m~0n", new JsonValue(8));
				// Url
				yield return new TestCaseData("#", (JsonValue) new JsonObject
					{
						["foo"] = new JsonArray {"bar", "baz"},
						[""] = 0,
						["a/b"] = 1,
						["c%d"] = 2,
						["e^f"] = 3,
						["g|h"] = 4,
						["i\\j"] = 5,
						["k\"l"] = 6,
						[" "] = 7,
						["m~n"] = 8,
					});
				yield return new TestCaseData("#/foo", (JsonValue) new JsonArray{"bar", "baz"});
				yield return new TestCaseData("#/foo/0", new JsonValue("bar"));
				yield return new TestCaseData("#/", new JsonValue(0));
				yield return new TestCaseData("#/a~1b", new JsonValue(1));
				yield return new TestCaseData("#/c%25d", new JsonValue(2));
				yield return new TestCaseData("#/e%5Ef", new JsonValue(3));
				yield return new TestCaseData("#/g%7Ch", new JsonValue(4));
				yield return new TestCaseData("#/i%5Cj", new JsonValue(5));
				yield return new TestCaseData("#/k%22l", new JsonValue(6));
				yield return new TestCaseData("#/%20", new JsonValue(7));
				yield return new TestCaseData("#/m~0n", new JsonValue(8));
			}
		}

		[TestCaseSource(nameof(ExampleCases))]
		public void Example(string pointerString, JsonValue expected)
		{
			var target = new JsonObject
				{
					["foo"] = new JsonArray {"bar", "baz"},
					[""] = 0,
					["a/b"] = 1,
					["c%d"] = 2,
					["e^f"] = 3,
					["g|h"] = 4,
					["i\\j"] = 5,
					["k\"l"] = 6,
					[" "] = 7,
					["m~n"] = 8,
				};

			var pointer = JsonPointer.Parse(pointerString);

			var actual = pointer.Evaluate(target);

			if (actual.Error != null)
				Console.WriteLine(actual.Error);

			Assert.AreEqual(expected, actual.Result);
		}
	}
}
