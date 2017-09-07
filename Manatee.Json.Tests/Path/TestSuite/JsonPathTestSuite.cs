using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Path.TestSuite
{
	[TestFixture]
	public class JsonPathTestSuite
	{
		private const string TestFolder = @"..\..\..\Json-Path-Test-Suite\tests\";
#pragma warning disable 649
		private const string FileNameForDebugging = "";
		private const string TestPathForDebugging = "";
#pragma warning restore 649
		private static readonly JsonSerializer Serializer;
		private int _failures;
		private int _passes;

		static JsonPathTestSuite()
		{
			Serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
		}

		[Test]
		public void RunSuite()
		{
			// uncomment and paste the filename of a test suite to debug it.
			//_fileNameForDebugging = "";
			// uncomment and paste the description of a test to debug it.
			//_testNameForDebugging = "ref within ref valid";

			try
			{
				var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, TestFolder).AdjustForOS();
				var fileNames = Directory.GetFiles(testsPath);

				foreach (var fileName in fileNames)
				{
					_RunFile(fileName);
				}

				Assert.AreEqual(0, _failures);
			}
			finally
			{
				Console.WriteLine();
				Console.WriteLine($"Passes: {_passes}");
				Console.WriteLine($"Failures: {_failures}");
			}
		}

		private void _RunFile(string fileName)
		{
			if (fileName == FileNameForDebugging)
			{
				System.Diagnostics.Debugger.Break();
			}

			var contents = File.ReadAllText(fileName);
			var json = JsonValue.Parse(contents);

			var testSet = Serializer.Deserialize<PathTestSet>(json);

			Console.WriteLine($"{testSet.Title} ({fileName})");
			_RunTestSet(testSet);
		}

		private void _RunTestSet(PathTestSet testSet)
		{
			foreach (var test in testSet.Tests)
			{
				var pathString = test.Path.ToString();
				if (pathString == TestPathForDebugging)
				{
					System.Diagnostics.Debugger.Break();
				}

				var results = test.Path.Evaluate(testSet.Data);

				if (Equals(results, test.Result))
				{
					// It's difficult to see the failures when everything shows.
					// The Stack Trace Explorer needs to show color output. :/
					//Console.WriteLine($"  {test.Description} - Passed");
					_passes++;
				}
				else
				{
					Console.WriteLine($"  {pathString} - Failed");
					_failures++;
				}
			}
		}
	}
}
