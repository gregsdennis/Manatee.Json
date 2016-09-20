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
 
	File Name:		JsonPathTestSuite.cs
	Namespace:		Manatee.Json.Tests.Schema.TestSuite
	Class Name:		JsonPathTestSuite
	Purpose:		Runs the series of schema tests defined at
					https://github.com/json-schema-org/JSON-Schema-Test-Suite.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path.TestSuite
{
	[TestClass]
	public class JsonPathTestSuite
	{
		private const string TestFolder = @"http://github.com/gregsdennis/Json-Path-Test-Suite/trunk/tests/";
		private static readonly JsonSerializer Serializer;
		private int _failures;
		private int _passes;
#pragma warning disable 649
		private string _fileNameForDebugging;
		private string _testPathForDebugging = "";
#pragma warning restore 649

		static JsonPathTestSuite()
		{
			Serializer = new JsonSerializer();
			JsonSerializationAbstractionMap.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
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

			var testSet = Serializer.Deserialize<PathTestSet>(json);

			Console.WriteLine($"{testSet.Title} ({fileName})");
			_RunTestSet(testSet);
		}

		private void _RunTestSet(PathTestSet testSet)
		{
			foreach (var test in testSet.Tests)
			{
				var pathString = test.Path.ToString();
				if (pathString == _testPathForDebugging)
				{
					System.Diagnostics.Debugger.Break();
				}

				var results = test.Path.Evaluate(testSet.Data);

				if (results == test.Result)
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
