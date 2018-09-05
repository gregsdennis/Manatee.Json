using System.Collections.Generic;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public interface IJsonSchemaKeyword : IJsonSerializable
	{
		string Name { get; }
		JsonSchemaVersion SupportedVersions { get; }

		SchemaValidationResults Validate(SchemaValidationContext context);
	}
}