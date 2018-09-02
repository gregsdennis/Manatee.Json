using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class RefKeyword : JsonSchemaKeyword
	{
		public static readonly RefKeyword Root = new RefKeyword("#");

		public string Name => "$ref";
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public string Reference { get; private set; }

		public JsonSchema Resolved { get; private set; }

		public RefKeyword(string reference)
		{
			Reference = reference;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return StringFormat.Uri.Validate(Reference)
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, "should be a valid URI");
		}

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Reference = json.String;
		}

		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Reference;
		}
	}
}