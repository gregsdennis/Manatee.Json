using System.Collections;
using Manatee.Json.Patch;
using NUnit.Framework;

namespace Manatee.Json.Tests.Patch
{
    [TestFixture]
    public class JsonPointerResolutionTests
    {
        public static IEnumerable TestData =>
            new[]
                {
                    new TestCaseData((JsonValue) new JsonObject {["a"] = new JsonObject {["b"] = new JsonObject {["c"] = 1}}},
                                     "/a/b/c",
                                     (JsonValue) new JsonObject {["c"] = 1},
                                     (JsonValue) 1),
                    new TestCaseData((JsonValue) new JsonObject {["a"] = new JsonObject {["b"] = new JsonObject {["c"] = 1}}},
                                     "/a/b",
                                     (JsonValue) new JsonObject {["b"] = new JsonObject {["c"] = 1}},
                                     (JsonValue) new JsonObject {["c"] = 1}),
                    new TestCaseData((JsonValue) new JsonObject {["a"] = new JsonArray {new JsonObject {["c"] = 1}}},
                                     "/a/0/c",
                                     (JsonValue) new JsonObject {["c"] = 1},
                                     (JsonValue) 1),
                    new TestCaseData((JsonValue) new JsonObject {["a"] = new JsonArray {new JsonObject {["c"] = 1}}},
                                     "/a/0",
                                     (JsonValue) new JsonArray {new JsonObject {["c"] = 1}},
                                     (JsonValue) new JsonObject {["c"] = 1}),
                    new TestCaseData((JsonValue) new JsonObject {["a"] = new JsonArray {new JsonObject {["c"] = 1}}},
                                     "/a/1/c",
                                     (JsonValue) null,
                                     (JsonValue) null),
                };
        
        [TestCaseSource(nameof(TestData))]
        public void Run(JsonValue json, string path, JsonValue expectedTarget, JsonValue expectedValue)
        {
            var (actualTarget, _, _, actualValue) = JsonPointerFunctions.ResolvePointer(json, path);
            
            Assert.AreEqual(expectedTarget, actualTarget);
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}