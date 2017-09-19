using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace Manatee.Json.Patch
{
    public class JsonPatch : List<JsonPatchAction>, IJsonSerializable
    {
        public static readonly JsonSchema04 Schema = new JsonSchema04
            {
                Title = "JSON schema for JSONPatch files",
                Id = "http://json.schemastore.org/json-patch#",
                Schema = JsonSchema04.MetaSchema.Id,
                Type = JsonSchemaTypeDefinition.Array,
                Items = new JsonSchemaReference("#/definitions/operation", typeof(JsonSchema04)),
                Definitions = new JsonSchemaTypeDefinitionCollection
                    {
                        ["operation"] = new JsonSchema04
                            {
                                Type = JsonSchemaTypeDefinition.Object,
                                Properties = new JsonSchemaPropertyDefinitionCollection
                                    {
                                        new JsonSchemaPropertyDefinition("op") {IsHidden = true, IsRequired = true},
                                        new JsonSchemaPropertyDefinition("path") {IsHidden = true, IsRequired = true},
                                    },
                                AllOf = new[] {new JsonSchemaReference("#/definitions/path", typeof(JsonSchema04))},
                                OneOf = new[]
                                    {
                                        new JsonSchema04
                                            {
                                                Properties = new JsonSchemaPropertyDefinitionCollection
                                                    {
                                                        new JsonSchemaPropertyDefinition("op")
                                                            {
                                                                Type = new JsonSchema04
                                                                    {
                                                                        Description = "The operation to perform.",
                                                                        Type = JsonSchemaTypeDefinition.String,
                                                                        Enum = new List<EnumSchemaValue> {"add", "replace", "test"}
                                                                    }
                                                            },
                                                        new JsonSchemaPropertyDefinition("value")
                                                            {
                                                                IsRequired = true,
                                                                Type = new JsonSchema04 {Description = "The value to add, replace or test."}
                                                            }
                                                    }
                                            },
                                        new JsonSchema04
                                            {
                                                Properties = new JsonSchemaPropertyDefinitionCollection
                                                    {
                                                        ["op"] = new JsonSchema04
                                                            {
                                                                Description = "The operation to perform.",
                                                                Type = JsonSchemaTypeDefinition.String,
                                                                Enum = new List<EnumSchemaValue> {"remove"}
                                                            }
                                                    }
                                            },
                                        new JsonSchema04
                                            {
                                                Properties = new JsonSchemaPropertyDefinitionCollection
                                                    {
                                                        new JsonSchemaPropertyDefinition("op")
                                                            {
                                                                Type = new JsonSchema04
                                                                    {
                                                                        Description = "The operation to perform.",
                                                                        Type = JsonSchemaTypeDefinition.String,
                                                                        Enum = new List<EnumSchemaValue> {"move", "copy"}
                                                                    }
                                                            },
                                                        new JsonSchemaPropertyDefinition("from")
                                                            {
                                                                Type = new JsonSchema04
                                                                    {
                                                                        Description = "A JSON Pointer path pointing to the location to move/copy from.",
                                                                        Type = JsonSchemaTypeDefinition.String
                                                                    },
                                                                IsRequired = true
                                                            }
                                                    }
                                            },
                                    }
                            },
                        ["path"] = new JsonSchema04
                            {
                                Properties = new JsonSchemaPropertyDefinitionCollection
                                    {
                                        ["path"] = new JsonSchema04
                                            {
                                                Description = "A JSON Pointer path.",
                                                Type = JsonSchemaTypeDefinition.String
                                            }
                                    }
                            }
                    }
            };
        
        public JsonPatchResult TryApply(JsonValue json)
        {
            var current = new JsonValue(json);
            var result = new JsonPatchResult(json);
            foreach (var action in this)
            {
                result = action.TryApply(current);
                if (result.Success)
                    current = result.Patched;
                else break;
            }
            return result;
        }
        
        public void FromJson(JsonValue json, JsonSerializer serializer)
        {
            AddRange(json.Array.Select(serializer.Deserialize<JsonPatchAction>));
        }
        public JsonValue ToJson(JsonSerializer serializer)
        {
            return new JsonArray(this.Select(pa => pa.ToJson(serializer)));
        }
    }
}