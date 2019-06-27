using System;
using Manatee.Json.Pointer;
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
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		JsonSchemaVersion SupportedVersions { get; }

		/// <summary>
		/// Provides the validation logic for this dependency.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		SchemaValidationResults Validate(SchemaValidationContext context);
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$ref` keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		/// <implementationNotes>
		/// If the dependency does not contain any schemas (e.g. `maximum`), this method is a no-op.
		/// </implementationNotes>
		void RegisterSubschemas(Uri baseUri);
		/// <summary>
		/// Resolves any subschemas during resolution of a `$ref` during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		/// <implementationNotes>
		/// If the dependency contains no subschemas, simply return null.
		/// If the dependency contains a subschema, simply pass the call to <see cref="JsonSchema.ResolveSubschema(JsonPointer, Uri)"/>.
		/// </implementationNotes>
		JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri);
	}
}
