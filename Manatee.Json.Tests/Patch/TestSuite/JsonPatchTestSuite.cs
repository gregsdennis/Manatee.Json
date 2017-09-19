using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Patch.TestSuite
{
    [TestFixture]
    public class JsonPatchTestSuite
    {
        private const string TestFolder = @"..\..\..\json-patch-tests";
        private static readonly JsonSerializer _serializer;

        public static IEnumerable TestData => _LoadTests();

        private static IEnumerable<TestCaseData> _LoadTests()
        {
            var testsPath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, TestFolder).AdjustForOS();
            var fileNames = Directory.GetFiles(testsPath, "*tests*.json");

            foreach (var fileName in fileNames)
            {
                var contents = File.ReadAllText(fileName);
                var json = JsonValue.Parse(contents);

                foreach (var test in json.Array)
                {
                    var testDescription = test.Object.TryGetString("comment") ?? "UNNAMED TEST";
                    var testName = testDescription.Replace(' ', '-');
                    yield return new TestCaseData(fileName, test) {TestName = testName};
                }
            }
        }

        static JsonPatchTestSuite()
        {
            _serializer = new JsonSerializer();
        }

        [TestCaseSource(nameof(TestData))]
        public void Run(string fileName, JsonValue testJson)
        {
            try
            {
                Console.WriteLine(fileName);
                Console.WriteLine(testJson.GetIndentedString());

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

                var result = test.Patch.TryApply(test.Doc);

                if (test.ExpectsError)
                    Assert.IsFalse(result.Success);
                else
                    Assert.AreEqual(test.Expected, result.Patched);
            }
            catch (Exception e)
            {
                if (testJson.Object.TryGetBoolean("disabled") ?? false)
                    Assert.Inconclusive();
                throw;
            }
        }
    }
}