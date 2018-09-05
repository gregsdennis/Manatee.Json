using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	[TestFixture]
	public class JsonSchemaTestSuite
	{
		private const string Draft04TestFolder = @"..\..\..\Json-Schema-Test-Suite\tests\draft4\";
		private const string Draft06TestFolder = @"..\..\..\Json-Schema-Test-Suite\tests\draft6\";
		private const string Draft07TestFolder = @"..\..\..\Json-Schema-Test-Suite\tests\draft7\";
		private const string Draft08TestFolder = @"..\..\..\Json-Schema-Test-Suite\tests\draft8\";
		private const string RemotesFolder = @"..\..\..\Json-Schema-Test-Suite\remotes\";
		private static readonly JsonSerializer _serializer;

		public static IEnumerable TestData04 => _LoadSchemaJson(Draft04TestFolder);
		public static IEnumerable TestData06 => _LoadSchemaJson(Draft06TestFolder);
		public static IEnumerable TestData07 => _LoadSchemaJson(Draft07TestFolder);
		public static IEnumerable TestData08 => _LoadSchemaJson(Draft08TestFolder);

		private static IEnumerable<TestCaseData> _LoadSchemaJson(string testFolder)
		{
			var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, testFolder).AdjustForOS();
			var fileNames = Directory.GetFiles(testsPath, "*.json");

			foreach (var fileName in fileNames)
			{
				var contents = File.ReadAllText(fileName);
				var json = JsonValue.Parse(contents);

				foreach (var testSet in json.Array)
				{
					var schemaJson = testSet.Object["schema"];
					foreach (var testJson in testSet.Object["tests"].Array)
					{
						var testName = $"{testSet.Object["description"]}.{testJson.Object["description"]}".Replace(' ', '_');
						yield return new TestCaseData(fileName, testJson, schemaJson, testName) {TestName = testName};
					}
				}
			}
		}
		
		static JsonSchemaTestSuite()
		{
			_serializer = new JsonSerializer();
		}

		[OneTimeSetUp]
		public static void Setup()
		{
			const string baseUri = "http://localhost:1234/";

			JsonSchemaOptions.Download = uri =>
				{
					var localPath = uri.Replace(baseUri, string.Empty);

					if (!Uri.TryCreate(localPath, UriKind.Absolute, out Uri newPath))
					{
						var remotesPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, RemotesFolder).AdjustForOS();
						newPath = new Uri(new Uri(remotesPath), localPath);
					}

					return File.ReadAllText(newPath.LocalPath);
				};
		}

		[OneTimeTearDown]
		public static void TearDown()
		{
			JsonSchemaOptions.Download = null;
		}

		//[TestCaseSource(nameof(TestData04))]
		//[TestCaseSource(nameof(TestData06))]
		//[TestCaseSource(nameof(TestData07))]
		//[TestCaseSource(nameof(TestData08))]
		public void Run(string fileName, JsonValue testJson, JsonValue schemaJson, string testName)
		{
			_Run(fileName, testJson, schemaJson);
		}

		private static void _Run(string fileName, JsonValue testJson, JsonValue schemaJson)
		{
			try
			{
				var test = _serializer.Deserialize<SchemaTest>(testJson);
				var schema = _serializer.Deserialize<JsonSchema>(schemaJson);
				var results = schema.Validate(test.Data);

				if (test.Valid != results.IsValid)
					Console.WriteLine(string.Join("\n", results.Errors));
				Assert.AreEqual(test.Valid, results.IsValid);

			}
			catch (Exception)
			{
				Console.WriteLine(fileName);
				throw;
			}
		}
	}
}
