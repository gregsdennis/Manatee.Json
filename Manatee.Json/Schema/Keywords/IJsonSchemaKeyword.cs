using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public interface IJsonSchemaKeyword : IJsonSerializable
	{
		string Name { get; }
		JsonSchemaVersion SupportedVersions { get; }
		int ValidationSequence { get; }

		SchemaValidationResults Validate(SchemaValidationContext context);
	}
}