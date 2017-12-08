using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Path;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Path.TestSuite
{
	[TestFixture]
	public class JsonPathTestSuite
	{
		private const string TestFolder = @"..\..\..\Json-Path-Test-Suite\tests\";
		private static readonly JsonSerializer _serializer;

		public static IEnumerable TestData
		{
			get
			{
				var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, TestFolder).AdjustForOS();
				var fileNames = Directory.GetFiles(testsPath);

				foreach (var fileName in fileNames)
				{
					var contents = File.ReadAllText(fileName);
					var json = JsonValue.Parse(contents);

					var testSet = _serializer.Deserialize<PathTestSet>(json);

					foreach (var test in testSet.Tests)
					{
						yield return new TestCaseData(testSet.Data, test)
							{
								TestName = $"{testSet.Title} / {test.Path}"
							};
					}
				}
			}
		}

		static JsonPathTestSuite()
		{
			_serializer = new JsonSerializer();
		}

		[TestCaseSource(nameof(TestData))]
		public void Run(JsonValue data, PathTest test)
		{
			var results = test.Path.Evaluate(data);
			
			Assert.AreEqual(test.Result, results);
		}
	}
}