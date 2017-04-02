using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	[TestClass]
	public class JsonSchemaTestSuite
	{
		private const string TestFolder = @"..\..\..\Json-Schema-Test-Suite\tests\draft4\";
		private const string RemotesFolder = @"..\..\..\Json-Schema-Test-Suite\remotes\";
		private static readonly Uri RemotesUri = new Uri(System.IO.Path.GetFullPath(RemotesFolder));
		private static readonly JsonSerializer _serializer;
		private int _failures;
		private int _passes;
#pragma warning disable 649
		private string _fileNameForDebugging;
		private string _testNameForDebugging;
#pragma warning restore 649

		static JsonSchemaTestSuite()
		{
			_serializer = new JsonSerializer();
		}

		[TestMethod]
		public void RunSuite()
		{
			// uncomment and paste the filename of a test suite to debug it.
			//_fileNameForDebugging = "";
			// uncomment and paste the description of a test to debug it.
			//_testNameForDebugging = "ref within ref valid";

			try
			{
				_StartServer();

				var fileNames = Directory.GetFiles(TestFolder);

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
			if (fileName == _fileNameForDebugging)
			{
				System.Diagnostics.Debugger.Break();
			}

			var contents = File.ReadAllText(fileName);
			var json = JsonValue.Parse(contents);

			var testSets = _serializer.Deserialize<List<SchemaTestSet>>(json);

			foreach (var testSet in testSets)
			{
				Console.WriteLine($"{testSet.Description} ({fileName})");
				_RunTestSet(testSet);
			}
		}

		private void _RunTestSet(SchemaTestSet testSet)
		{
			foreach (var test in testSet.Tests)
			{
				if (test.Description == _testNameForDebugging)
				{
					System.Diagnostics.Debugger.Break();
				}

				var results = testSet.Schema.Validate(test.Data);

				if (results.Valid == test.Valid)
				{
					// It's difficult to see the failures when everything shows.
					// The Stack Trace Explorer needs to show color output. :/
					//Console.WriteLine($"  {test.Description} - Passed");
					_passes++;
				}
				else
				{
					Console.WriteLine($"  {test.Description} - Failed");
					_failures++;
				}
			}
		}

		private static void _StartServer()
		{
			const string baseUri = "http://localhost:1234/";

			JsonSchemaOptions.Download = uri =>
				{
					var localPath = uri.Replace(baseUri, string.Empty);
					Uri newPath = null;

					if (!Uri.TryCreate(localPath, UriKind.Absolute, out newPath))
					{
						newPath = new Uri(RemotesUri, localPath);
					}

					return File.ReadAllText(newPath.LocalPath);
				};
		}
	}
}
