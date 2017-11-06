﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Patch;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Patch.TestSuite
{
	[TestFixture]
	public class JsonPatchTestSuite
	{
		private const string _testFolder = @"..\..\..\json-patch-tests";
		private static readonly JsonSerializer _serializer;

		// ReSharper disable once MemberCanBePrivate.Global
		public static IEnumerable TestData => _LoadTests();

		private static IEnumerable<TestCaseData> _LoadTests()
		{
			JsonOptions.DuplicateKeyBehavior = DuplicateKeyBehavior.Overwrite;

			var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, _testFolder).AdjustForOS();
			var fileNames = Directory.GetFiles(testsPath, "*tests*.json");

			foreach (var fileName in fileNames)
			{
				var contents = File.ReadAllText(fileName);
				var json = JsonValue.Parse(contents);

				foreach (var test in json.Array)
				{
					var testDescription = test.Object.TryGetString("comment") ?? "UNNAMED TEST";
					var testName = testDescription.Replace(' ', '_');
					yield return new TestCaseData(fileName, test) {TestName = testName};
				}
			}

			JsonOptions.DuplicateKeyBehavior = DuplicateKeyBehavior.Throw;
		}

		static JsonPatchTestSuite()
		{
			_serializer = new JsonSerializer();
		}

		[TestCaseSource(nameof(TestData))]
		public void Run(string fileName, JsonValue testJson)
		{
			JsonPatchResult result = null;
			try
			{
				var schemaValidation = JsonPatchTest.Schema.Validate(testJson);
				if (!schemaValidation.Valid)
				{
					foreach (var error in schemaValidation.Errors)
					{
						Console.WriteLine(error);
					}
					return;
				}
				var test = _serializer.Deserialize<JsonPatchTest>(testJson);

				result = test.Patch.TryApply(test.Doc);

				Assert.AreNotEqual(test.ExpectsError, result.Success);
				if (test.HasExpectedValue)
					Assert.AreEqual(test.ExpectedValue, result.Patched);
			}
			catch (Exception e)
			{
				Console.WriteLine(fileName);
				Console.WriteLine(testJson.GetIndentedString());
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				if (result != null)
					Console.WriteLine(result.Error);
				if (testJson.Object.TryGetBoolean("disabled") ?? false)
					Assert.Inconclusive();
				throw;
			}
		}
	}
}