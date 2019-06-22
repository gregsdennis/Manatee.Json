using Manatee.Json.Patch;
using Manatee.Json.Schema;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Patch.TestSuite
{
	public class JsonPatchTest : IJsonSerializable
	{
		public static readonly JsonSchema Schema = new JsonSchema()
			.Type(JsonSchemaType.Object)
			.Property("doc", JsonSchema.Empty)
			.Property("expected", JsonSchema.Empty)
			.Property("patch", new JsonSchema().Ref(JsonPatch.Schema.Id))
			.Property("comment", new JsonSchema().Type(JsonSchemaType.String))
			.Property("error", new JsonSchema().Type(JsonSchemaType.String))
			.Property("disabled", new JsonSchema().Type(JsonSchemaType.Boolean));
		
		public JsonValue Doc { get; set; }
		public JsonValue ExpectedValue { get; set; }
		public bool HasExpectedValue { get; set; }
		public bool ExpectsError { get; set; }
		public string Comment { get; set; }
		public JsonPatch Patch { get; set; }
		public bool Disabled { get; set; }
		
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Comment = obj.TryGetString("comment");
			Doc = obj["doc"];
			if (obj.ContainsKey("expected"))
				ExpectedValue = obj["expected"];
			else
				HasExpectedValue = false;
			ExpectsError = !string.IsNullOrWhiteSpace(obj.TryGetString("error"));
			Patch = serializer.Deserialize<JsonPatch>(obj["patch"]);
			Disabled = obj.TryGetBoolean("disabled") ?? false;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			throw new System.NotImplementedException();
		}
	}
}