using System;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a keyword for JSON Schema.
	/// </summary>
	public interface IJsonSchemaKeyword : IJsonSerializable, IEquatable<IJsonSchemaKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		JsonSchemaVersion SupportedVersions { get; }
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		/// <implementationNotes>
		/// May be duplicated across different keywords.  This property comes into play when there
		/// are several keywords which must be evaluated in a specific order.
		/// </implementationNotes>
		int ValidationSequence { get; }

		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		SchemaVocabulary Vocabulary { get; }

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		SchemaValidationResults Validate(SchemaValidationContext context);
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with <code>$ref</code> keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		/// <implementationNotes>
		/// If the keyword does not contain any schemas (e.g. <code>maximum</code>), this method is a no-op.
		/// </implementationNotes>
		void RegisterSubschemas(Uri baseUri);
		/// <summary>
		/// Resolves any subschemas during resolution of a <code>$ref</code> during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		/// <implementationNotes>
		/// If the keyword contains no subschemas, simply return null.
		/// If the keyword contains a subschema, simply pass the call to <see cref="JsonSchema.ResolveSubschema(JsonPointer, Uri)"/>.
		/// </implementationNotes>
		JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri);
	}

	internal interface IJsonSchemaKeywordPlus : IJsonSchemaKeyword
	{
		bool Handles(JsonValue value);
	}
}