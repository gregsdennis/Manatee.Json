using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Manatee.Json.Patch;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;
using NUnit.Framework;

namespace Manatee.Json.Tests.Patch.TestSuite
{
    public class JsonPatchTest : IJsonSerializable
    {
        public static readonly IJsonSchema Schema = new JsonSchema04
            {
                Type = JsonSchemaTypeDefinition.Object,
                Properties = new JsonSchemaPropertyDefinitionCollection
                    {
                        ["doc"] = JsonSchema04.Empty,
                        ["expected"] = JsonSchema04.Empty,
                        ["patch"] = new JsonSchemaReference(JsonPatch.Schema.Id, typeof(JsonSchema04)),
                        ["comment"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String},
                        ["error"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String},
                        ["disabled"] = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Boolean}
                    }
            };
        
        public JsonValue Doc { get; set; }
        public JsonValue Expected { get; set; }
        public bool ExpectsError { get; set; }
        public string Comment { get; set; }
        public JsonPatch Patch { get; set; }
        
        public void FromJson(JsonValue json, JsonSerializer serializer)
        {
            var obj = json.Object;
            Comment = obj.TryGetString("comment");
            Doc = obj["doc"];
            if (obj.ContainsKey("expected"))
                Expected = obj["expected"];
            ExpectsError = !string.IsNullOrWhiteSpace(obj.TryGetString("error"));
            Patch = serializer.Deserialize<JsonPatch>(obj["patch"]);
        }
        public JsonValue ToJson(JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }
    }

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