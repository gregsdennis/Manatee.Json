/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSchemaTestSuite.cs
	Namespace:		Manatee.Json.Tests.Schema.TestSuite
	Class Name:		JsonSchemaTestSuite
	Purpose:		Runs the series of schema tests defined at
					https://github.com/json-schema-org/JSON-Schema-Test-Suite.

***************************************************************************************/
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
		private static readonly JsonSerializer _serializer;
		private int _failures;
		private int _passes;
		private string _fileNameForDebugging;
		private string _testNameForDebugging;

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
			//_testNameForDebugging = "remote ref valid";

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

		private void _StartServer()
		{
			const string baseUri = "http://localhost:1234/";

			JsonSchemaOptions.Download = uri =>
				{
					var localPath = uri.Replace(baseUri, string.Empty);
					var newPath = System.IO.Path.Combine(RemotesFolder, localPath);
					return File.ReadAllText(newPath);
				};
		}
	}
}
