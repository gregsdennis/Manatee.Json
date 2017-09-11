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
		private const string TestFolder = @"..\..\..\Json-Schema-Test-Suite\tests\draft4\";
		private const string RemotesFolder = @"..\..\..\Json-Schema-Test-Suite\remotes\";
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

					var testSets = _serializer.Deserialize<List<SchemaTestSet>>(json);

					foreach (var testSet in testSets)
					{
						foreach (var test in testSet.Tests)
						{
							yield return new TestCaseData(testSet.Schema, test, fileName)
								{
									TestName = $"{testSet.Description} / {test.Description}"
								};
						}
					}
				}
			}
		}
		
		static JsonSchemaTestSuite()
		{
			_serializer = new JsonSerializer();
			_StartServer();
		}

		[TestCaseSource(nameof(TestData))]
		public void Run(IJsonSchema schema, SchemaTest test, string fileName)
		{
			var results = schema.Validate(test.Data);

			if (!results.Valid)
			{
				Console.WriteLine(fileName);
				Console.WriteLine(string.Join("\n", results.Errors));
			}
			Assert.AreEqual(test.Valid, results.Valid);
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
						var remotesPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, RemotesFolder).AdjustForOS();
						newPath = new Uri(new Uri(remotesPath), localPath);
					}

					return File.ReadAllText(newPath.LocalPath);
				};
		}
	}
}
