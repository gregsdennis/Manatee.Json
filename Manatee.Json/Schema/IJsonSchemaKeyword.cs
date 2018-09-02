using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public interface JsonSchemaKeyword : IJsonSerializable
	{
		string Name { get; }
		JsonSchemaVersion SupportedVersions { get; }

		SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json);
	}
}