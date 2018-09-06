using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public interface IJsonSchemaKeyword : IJsonSerializable, IEquatable<IJsonSchemaKeyword>
	{
		string Name { get; }
		JsonSchemaVersion SupportedVersions { get; }
		int ValidationSequence { get; }

		SchemaValidationResults Validate(SchemaValidationContext context);
	}

	internal interface IJsonSchemaKeywordPlus : IJsonSchemaKeyword
	{
		bool Handles(JsonValue value);
	}

	internal interface IResolvePointers
	{
		JsonSchema Resolve(string property);
	}
}