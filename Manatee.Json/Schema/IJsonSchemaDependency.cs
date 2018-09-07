using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines properties and methods required to represent dependencies within JSON Schema.
	/// </summary>
	public interface IJsonSchemaDependency : IJsonSerializable, IEquatable<IJsonSchemaDependency>
	{
		/// <summary>
		/// Gets or sets the property with the dependency.
		/// </summary>
		string PropertyName { get; }
		JsonSchemaVersion SupportedVersions { get; }

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		SchemaValidationResults Validate(SchemaValidationContext context);
	}
}
