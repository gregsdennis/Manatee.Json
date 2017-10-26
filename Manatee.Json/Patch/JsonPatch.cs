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
								Required = new List<string> {"op", "path"},
								AllOf = new[] {new JsonSchemaReference("#/definitions/path", typeof(JsonSchema04))},
								OneOf = new[]
									{
										new JsonSchema04
											{
												Properties = new Dictionary<string, IJsonSchema>
													{
														["op"] = new JsonSchema04
															{
																Description = "The operation to perform.",
																Type = JsonSchemaTypeDefinition.String,
																Enum = new List<EnumSchemaValue> {"add", "replace", "test"}
															},
														["value"] = new JsonSchema04 {Description = "The value to add, replace or test."}
													},
												Required = new[] {"value"}
											},
										new JsonSchema04
											{
												Properties = new Dictionary<string, IJsonSchema>
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
												Properties = new Dictionary<string, IJsonSchema>
													{
														["op"] = new JsonSchema04
															{
																Description = "The operation to perform.",
																Type = JsonSchemaTypeDefinition.String,
																Enum = new List<EnumSchemaValue> {"move", "copy"}
															},
														["from"] = new JsonSchema04
															{
																Description = "A JSON Pointer path pointing to the location to move/copy from.",
																Type = JsonSchemaTypeDefinition.String
															},
													},
												Required = new[] {"from"}
											}
									}
							},
						["path"] = new JsonSchema04
							{
								Properties = new Dictionary<string, IJsonSchema>
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

		void IJsonSerializable.FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(serializer.Deserialize<JsonPatchAction>));
		}
		JsonValue IJsonSerializable.ToJson(JsonSerializer serializer)
		{
			return new JsonArray(this.Select(pa => ((IJsonSerializable) pa).ToJson(serializer)));
		}
	}
}