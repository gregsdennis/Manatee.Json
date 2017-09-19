using Manatee.Json.Patch;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

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
}