using System;
using Manatee.Json.Path;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Manatee.Json.Tests.Path
{
	/// <summary>
	/// These are a set of tests that GitHub user cburgmer uses to check all JSON Path implementations
	/// for feature support.  Adding the suite here ensures that I support them all.
	/// </summary>
	[TestFixture]
	public class CburgmerFeatureValidationTests
	{
		private const string RegressionResultsFile = @"..\..\..\..\json-path-comparison\regression_suite\regression_suite.yaml";
		private static readonly Regex IdPattern = new Regex(@"  - id: (?<value>.*)");
		private static readonly Regex SelectorPattern = new Regex(@"    selector: (?<value>.*)");
		private static readonly Regex DocumentPattern = new Regex(@"    document: (?<value>.*)");
		private static readonly Regex ConsensusPattern = new Regex(@"    consensus: (?<value>.*)");

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
				var fileLines = File.ReadAllLines(RegressionResultsFile);
				CburgmerTestCase currentTestCase = null;
				foreach (var line in fileLines)
				{
					if (TryMatch(line, IdPattern, out var value))
					{
						if (currentTestCase != null)
							yield return new TestCaseData(currentTestCase) {TestName = currentTestCase.TestName};
						currentTestCase = new CburgmerTestCase{TestName = value};
					}
					else if (TryMatch(line, SelectorPattern, out value))
					{
						currentTestCase.PathString = value;
					}
					else if (TryMatch(line, DocumentPattern, out value))
					{
						currentTestCase.JsonString = value;
					}
					else if (TryMatch(line, ConsensusPattern, out value))
					{
						currentTestCase.ExpectedResultString = value;
					}
				}
			}
		}

		[TestCaseSource(nameof(TestCases))]
		public void Run(CburgmerTestCase testCase)
		{
			Console.WriteLine(testCase);
			Console.WriteLine();

			if (testCase.ExpectedResultString == null)
				Assert.Inconclusive("Test case has no consensus result.  Cannot validate.");
			
			var expected = JsonValue.Parse(testCase.ExpectedResultString);

			var actual = Evaluate(testCase.JsonString, testCase.PathString);

			Assert.AreEqual(expected, actual);
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
		public string ExpectedResultString { get; set; }

		public override string ToString()
		{
			return $"TestName:   {TestName}\n" +
				   $"PathString: {PathString}\n" +
				   $"JsonString: {JsonString}\n" +
				   $"Expected:   {ExpectedResultString}";
		}
	}
}
