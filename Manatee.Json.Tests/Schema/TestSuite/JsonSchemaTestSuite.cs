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
		private const string RemotesFolder = @"..\..\..\Json-Schema-Test-Suite\remotes\";
		private static readonly JsonSerializer _serializer;

		public static IEnumerable TestData => _LoadSchema<JsonSchema04>(Draft04TestFolder).Concat(_LoadSchema<JsonSchema06>(Draft06TestFolder));

		private static IEnumerable<TestCaseData> _LoadSchema<T>(string testFolder)
			where T : IJsonSchema
		{
			var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, testFolder).AdjustForOS();
			var fileNames = Directory.GetFiles(testsPath, "*.json");

			JsonSchemaFactory.SetDefaultSchemaVersion<T>();
			var schemata = new List<TestCaseData>();
			foreach (var fileName in fileNames)
			{
				var contents = File.ReadAllText(fileName);
				var json = JsonValue.Parse(contents);

				var testSets = _serializer.Deserialize<List<SchemaTestSet>>(json);

				foreach (var testSet in testSets)
				{
					foreach (var test in testSet.Tests)
					{
						schemata.Add(new TestCaseData(testSet.Schema, test, fileName)
							{
								TestName = $"{testSet.Description}.{test.Description}".Replace(' ', '_')
							});
					}
				}
			}
			return schemata;
		}

		static JsonSchemaTestSuite()
		{
			_serializer = new JsonSerializer();
		}

		[OneTimeSetUp]
		public void Setup()
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
		public void TearDown()
		{
			JsonSchemaOptions.Download = null;
		}

		[TestCaseSource(nameof(TestData))]
		public void Run(IJsonSchema schema, SchemaTest test, string fileName)
		{
			Console.WriteLine(fileName);
			
			var results = schema.Validate(test.Data);

			if (test.Valid != results.Valid)
				Console.WriteLine(string.Join("\n", results.Errors));
			Assert.AreEqual(test.Valid, results.Valid);
		}
	}
}
