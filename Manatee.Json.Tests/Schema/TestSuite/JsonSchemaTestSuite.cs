using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
		private const string RootTestsFolder = @"..\..\..\..\Json-Schema-Test-Suite\tests\";
		private const string RemotesFolder = @"..\..\..\..\Json-Schema-Test-Suite\remotes\";
		private static readonly JsonSerializer _serializer;

		public static IEnumerable AllTestData => _LoadSchemaJson("draft4")
			.Concat(_LoadSchemaJson("draft6"))
			.Concat(_LoadSchemaJson("draft7"))
			.Concat(_LoadSchemaJson("draft8"));

		private static IEnumerable<TestCaseData> _LoadSchemaJson(string draft)
		{
			var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.WorkDirectory, RootTestsFolder, $"{draft}\\").AdjustForOS();
			if (!Directory.Exists(testsPath)) yield break;

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
						var testName = $"{testSet.Object["description"]}.{testJson.Object["description"]}.{draft}".Replace(' ', '_');
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

		[TestCaseSource(nameof(AllTestData))]
		public void Run(string fileName, JsonValue testJson, JsonValue schemaJson, string testName)
		{
			var outputFormat = JsonSchemaOptions.OutputFormat;
			JsonSchemaOptions.OutputFormat = SchemaValidationOutputFormat.Flag;
			try
			{
				_Run(fileName, testJson, schemaJson);
			}
			finally
			{
				JsonSchemaOptions.OutputFormat = outputFormat;
			}
		}

		private static void _Run(string fileName, JsonValue testJson, JsonValue schemaJson)
		{
			try
			{
				var test = _serializer.Deserialize<SchemaTest>(testJson);
				var schema = _serializer.Deserialize<JsonSchema>(schemaJson);

				if (test.Description == "mismatch base schema")
				{
					Debugger.Break();
				}

				var results = schema.Validate(test.Data);

				if (test.Valid != results.IsValid)
					Console.WriteLine(string.Join("\n", _serializer.Serialize(results).GetIndentedString()));
				Assert.AreEqual(test.Valid, results.IsValid);

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
				throw;
			}
		}
	}
}
