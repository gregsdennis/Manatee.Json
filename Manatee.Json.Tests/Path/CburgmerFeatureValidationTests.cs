using System;
using Manatee.Json.Path;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Manatee.Json.Tests.Path
{
	/// <summary>
	/// These are a set of tests that GitHub user cburgmer uses to check all JSON Path implementations
	/// for feature support.  Adding the suite here ensures that I support them all.
	/// </summary>
	/// <remarks>This is from cburgmer's amazing reporting site: https://cburgmer.github.io/json-path-comparison/</remarks>
	[TestFixture]
	public class CburgmerFeatureValidationTests
	{
		private const string _regressionResultsFile = @"..\..\..\..\json-path-comparison\regression_suite\regression_suite.yaml";
		private static readonly Regex _idPattern = new Regex(@"  - id: (?<value>.*)");
		private static readonly Regex _selectorPattern = new Regex(@"    selector: ""(?<value>.*)""");
		private static readonly Regex _documentPattern = new Regex(@"    document: (?<value>.*)");
		private static readonly Regex _consensusPattern = new Regex(@"    consensus: (?<value>.*)");
		private static readonly string[] _notSupported =
			{
				// invalid
				"$[?(@.key=42)]",
				"$[?(@)]",
				"$['single'quote']",

				// nested expression
				"$[?(@.a[?(@.price>10)])]",
				
				// unions
				"$['key','another']",
				"$['one','three'].key",
				"$[?(@<3),?(@>6)]",
				"$[:]['c','d']",
				"$[0]['c','d']",
				"$.*['c','d']",

				// path within indexer
				"$['two'.'some']",
				"$[two.some]",

				// unquoted keys
				"$[key]",
				"$.[key]",
				
				// no search term
				"$..",

				// big numbers
				"$[2:113667776004]",
				"$[2:-113667776004:-1]",
				"$[-113667776004:2]",
				"$[113667776004:2:-1]",
				
				// maybe add support for this later
				"$..[?(@.id==2)]",
				"$..[?(@.id)]",
				"$[?(@.name=~/hello.*/)]",
			};

		private ArrayEquality _arrayEquality;

		//  - id: array_index
		//    selector: $[2]
		//    document: ["first", "second", "third", "forth", "fifth"]
		//    consensus: ["third"]
		//    scalar-consensus: "third"
		public static IEnumerable<TestCaseData> TestCases
		{
			get
			{
				bool TryMatch(string line, Regex pattern, out string value)
				{
					var match = pattern.Match(line);
					if (!match.Success)
					{
						value = null;
						return false;
					}

					value = match.Groups["value"].Value;
					return true;
				}

				// what I wouldn't give for a YAML parser...
				var fileLines = File.ReadAllLines(_regressionResultsFile);
				CburgmerTestCase currentTestCase = null;
				foreach (var line in fileLines)
				{
					if (TryMatch(line, _idPattern, out var value))
					{
						if (currentTestCase != null)
							yield return new TestCaseData(currentTestCase) {TestName = currentTestCase.TestName};
						currentTestCase = new CburgmerTestCase{TestName = value};
					}
					else if (TryMatch(line, _selectorPattern, out value))
					{
						currentTestCase.PathString = value;
					}
					else if (TryMatch(line, _documentPattern, out value))
					{
						currentTestCase.JsonString = value;
					}
					else if (TryMatch(line, _consensusPattern, out value))
					{
						currentTestCase.Consensus = value;
					}
				}
			}
		}

		[OneTimeSetUp]
		public void Setup()
		{
			_arrayEquality = JsonOptions.DefaultArrayEquality;
			JsonOptions.DefaultArrayEquality = ArrayEquality.ContentsEqual;
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			JsonOptions.DefaultArrayEquality = _arrayEquality;
		}

		[TestCaseSource(nameof(TestCases))]
		[Ignore("Ignoring these for now.  May revisit at a later date.")]
		public void Run(CburgmerTestCase testCase)
		{
			if (_notSupported.Contains(testCase.PathString))
				Assert.Inconclusive("This case will not be supported.");

			Console.WriteLine(testCase);
			Console.WriteLine();

			var actual = Evaluate(testCase.JsonString, testCase.PathString);

			if (testCase.Consensus == null)
			{
				Console.WriteLine($"Actual: {actual}");
				Assert.Inconclusive("Test case has no consensus result.  Cannot validate.");
			}
			else
			{
				var expected = JsonValue.Parse(testCase.Consensus);
				Assert.AreEqual(expected, actual);
			}
		}

		private JsonArray Evaluate(string jsonString, string pathString)
		{
			var o = JsonValue.Parse(jsonString);
			var selector = pathString;
			var path = JsonPath.Parse(selector);
			var results = path.Evaluate(o);

			return results;
		}
	}

	public class CburgmerTestCase
	{
		public string TestName { get; set; }
		public string PathString { get; set; }
		public string JsonString { get; set; }
		public string Consensus { get; set; }

		public override string ToString()
		{
			return $"TestName:   {TestName}\n" +
				   $"PathString: {PathString}\n" +
				   $"JsonString: {JsonString}\n" +
				   $"Consensus:   {Consensus}";
		}
	}
}
