using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private const string RemotesFolder = @"..\..\..\Json-Schema-Test-Suite\remotes\";
		private static readonly JsonSerializer _serializer;

		public static IEnumerable TestData04 => _LoadSchemaJson(Draft04TestFolder);
		public static IEnumerable TestData06 => _LoadSchemaJson(Draft06TestFolder);
		public static IEnumerable TestData07 => _LoadSchemaJson(Draft07TestFolder);

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

		[TestCaseSource(nameof(TestData04))]
		public void Run04(string fileName, JsonValue testJson, JsonValue schemaJson, string testname)
		{
			JsonSchemaFactory.SetDefaultSchemaVersion<JsonSchema04>();
			_Run<JsonSchema04>(fileName, testJson, schemaJson);
		}

		[TestCaseSource(nameof(TestData06))]
		public void Run06(string fileName, JsonValue testJson, JsonValue schemaJson, string testname)
		{
			JsonSchemaFactory.SetDefaultSchemaVersion<JsonSchema06>();
			_Run<JsonSchema06>(fileName, testJson, schemaJson);
		}

		[TestCaseSource(nameof(TestData07))]
		public void Run07(string fileName, JsonValue testJson, JsonValue schemaJson, string testname)
		{
			JsonSchemaFactory.SetDefaultSchemaVersion<JsonSchema07>();
			_Run<JsonSchema07>(fileName, testJson, schemaJson);
		}

		private static void _Run<T>(string fileName, JsonValue testJson, JsonValue schemaJson)
			where T : IJsonSchema
		{
			try
			{
				JsonSchemaFactory.SetDefaultSchemaVersion<T>();
				var test = _serializer.Deserialize<SchemaTest>(testJson);
				var schema = _serializer.Deserialize<IJsonSchema>(schemaJson);
				var results = schema.Validate(test.Data);

				if (test.Valid != results.Valid)
					Console.WriteLine(string.Join("\n", results.Errors));
				Assert.AreEqual(test.Valid, results.Valid);

			}
			catch (Exception)
			{
				Console.WriteLine(fileName);
				throw;
			}
		}

		[Test]
		[Ignore("Test for debugging purposes only")]
		public void RunOne()
		{
			var testName = "propertyNames_validation.some_property_names_invalid";

			var selectedTest = _LoadSchemaJson(Draft06TestFolder)
				.First(d => Equals(d.Arguments[3], testName));

			// ReSharper disable PossibleInvalidCastException
			_Run<JsonSchema06>((string) selectedTest.Arguments[0],
			                   (JsonValue) selectedTest.Arguments[0],
			                   (JsonValue) selectedTest.Arguments[0]);
			// ReSharper restore PossibleInvalidCastException
		}
	}
}
