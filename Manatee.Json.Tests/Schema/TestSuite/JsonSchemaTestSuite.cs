using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Manatee.Json.Tests.Schema.TestSuite
{
	[TestFixture]
	public class JsonSchemaTestSuite
	{
		private const string RootTestsFolder = @"..\..\..\..\Json-Schema-Test-Suite\tests\";
		private const string RemotesFolder = @"..\..\..\..\Json-Schema-Test-Suite\remotes\";
		private const string OutputFolder = @"..\..\..\..\Json-Schema-Test-Results\";
		private static readonly JsonSerializer _serializer;

		// these are tests that are considered required, but I can't support for whatever reason
		private static readonly string[] _notSupported =
			{

			};

		public static IEnumerable AllTestData => _LoadSchemaJson("draft4", JsonSchemaVersion.Draft04)
			.Concat(_LoadSchemaJson("draft6", JsonSchemaVersion.Draft06))
			.Concat(_LoadSchemaJson("draft7", JsonSchemaVersion.Draft07))
			.Concat(_LoadSchemaJson("draft2019-09", JsonSchemaVersion.Draft2019_09));

		private static IEnumerable<TestCaseData> _LoadSchemaJson(string draft, JsonSchemaVersion version)
		{
			var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.WorkDirectory, RootTestsFolder, $"{draft}\\").AdjustForOS();
			if (!Directory.Exists(testsPath)) return Enumerable.Empty<TestCaseData>();

			var fileNames = Directory.GetFiles(testsPath, "*.json", SearchOption.AllDirectories);

			var allTests = new List<TestCaseData>();
			foreach (var fileName in fileNames)
			{
				var shortFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
				var contents = File.ReadAllText(fileName);
				var json = JsonValue.Parse(contents);

				foreach (var testSet in json.Array)
				{
					var schemaJson = testSet.Object["schema"];
					foreach (var testJson in testSet.Object["tests"].Array)
					{
						var isOptional = fileName.Contains("optional");
						var testName = $"{shortFileName}.{testSet.Object["description"].String}.{testJson.Object["description"].String}.{draft}".Replace(' ', '_');
						if (isOptional)
							testName = $"optional.{testName}";
						allTests.Add(new TestCaseData(fileName, testSet.Object["description"].String, testJson, schemaJson, version, isOptional, testName)
							{
								TestName = testName
							});
					}
				}
			}

			return allTests;
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
			JsonOptions.LogCategory = LogCategory.Schema;

			if (Directory.Exists(OutputFolder))
				Directory.Delete(OutputFolder, true);
			Directory.CreateDirectory(OutputFolder);

			SchemaValidationResults.IncludeAdditionalInfo = false;

			var configureForTestOutputValue = Environment.GetEnvironmentVariable("EXPORT_JSON_TEST_SUITE_RESULTS");
			bool.TryParse(configureForTestOutputValue, out var configureForTestOutput);
			JsonSchemaOptions.ConfigureForTestOutput = configureForTestOutput;
		}

		[OneTimeTearDown]
		public static void TearDown()
		{
			JsonSchemaOptions.Download = null;
			JsonSchemaOptions.ConfigureForTestOutput = false;
			SchemaValidationResults.IncludeAdditionalInfo = true;
		}

		[TestCaseSource(nameof(AllTestData))]
		public void Run(string fileName, string setDescription, JsonValue testJson, JsonValue schemaJson, JsonSchemaVersion version, bool isOptional, string testName)
		{
			Console.WriteLine(testName);
			Console.WriteLine();

			//if (testName == "ref.escaped_pointer_ref.slash_invalid.draft2019-09")
			//	Debugger.Break();

			var outputFormat = JsonSchemaOptions.OutputFormat;
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Verbose;
			try
			{
				_Run(fileName, setDescription, testJson, schemaJson, version, isOptional);
			}
			finally
			{
				JsonSchemaOptions.OutputFormat = outputFormat;
			}
		}

		private static void _Run(string fileName, string setDescription, JsonValue testJson, JsonValue schemaJson, JsonSchemaVersion version, bool isOptional)
		{
			using (new TestExecutionContext.IsolatedContext())
			{
				try
				{
					var test = _serializer.Deserialize<SchemaTest>(testJson);
					var schema = _serializer.Deserialize<JsonSchema>(schemaJson);
					schema.ProcessingVersion = version;

					var results = schema.Validate(test.Data);

					if (test.Valid != results.IsValid)
						Console.WriteLine(string.Join("\n", _serializer.Serialize(results).GetIndentedString()));
					Assert.AreEqual(test.Valid, results.IsValid);
					if (test.Output != null)
					{
						Assert.AreEqual(test.Output.Basic, results.Flatten());
						Assert.AreEqual(test.Output.Detailed, results.Condense());
						Assert.AreEqual(test.Output.Verbose, results);
					}

					if (!fileName.Contains("draft2019-09")) return;
					
					var exportTestsValue = Environment.GetEnvironmentVariable("EXPORT_JSON_TEST_SUITE_RESULTS");

					if (!bool.TryParse(exportTestsValue, out var exportTests) || !exportTests) return;

					test.OutputGeneration = results;

					List<SchemaTestSet> testSets = null;
					SchemaTestSet testSet = null;
					var outputFile = System.IO.Path.Combine(OutputFolder, System.IO.Path.GetFileName(fileName));
					if (File.Exists(outputFile))
					{
						var fileContents = File.ReadAllText(outputFile);
						var fileJson = JsonValue.Parse(fileContents);
						testSets = _serializer.Deserialize<List<SchemaTestSet>>(fileJson);
						testSet = testSets.FirstOrDefault(s => s.Description == setDescription);
					}

					if (testSets == null)
						testSets = new List<SchemaTestSet>();

					if (testSet == null)
					{
						testSet = new SchemaTestSet
							{
								Description = setDescription,
								Schema = schema,
								Tests = new List<SchemaTest>()
							};
						testSets.Add(testSet);
					}

					testSet.Tests.Add(test);

					var outputJson = _serializer.Serialize(testSets);
					File.WriteAllText(outputFile, outputJson.GetIndentedString());
				}
				catch (Exception e)
				{
					if (e is SchemaLoadException sle)
						Console.WriteLine(sle.MetaValidation.ToJson(new JsonSerializer()).GetIndentedString());

					Console.WriteLine(fileName);
					Console.WriteLine("\nSchema");
					Console.WriteLine(schemaJson.GetIndentedString());
					Console.WriteLine("\nTest");
					Console.WriteLine(testJson.GetIndentedString());
					
					if (isOptional)
						Assert.Inconclusive("This is an acceptable failure.  Test case failed, but was marked as 'optional'.");
					throw;
				}
			}
		}
	}
}
