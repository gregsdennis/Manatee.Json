using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the <code>$defs</code> JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class DefsKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword, IEquatable<DefsKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public virtual string Name => "$defs";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft2019_04;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 1;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public virtual SchemaVocabulary Vocabulary => SchemaVocabularies.Core;

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return new SchemaValidationResults(Name, context);
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with <code>$ref</code> keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		public void RegisterSubschemas(Uri baseUri)
		{
			foreach (var schema in Values)
			{
				schema.RegisterSubschemas(baseUri);
			}
		}
		/// <summary>
		/// Resolves any subschemas during resolution of a <code>$ref</code> during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			var first = pointer.FirstOrDefault();
			if (first == null) return null;

			if (!TryGetValue(first, out var schema)) return null;

			return schema.ResolveSubschema(new JsonPointer(pointer.Skip(1)), baseUri);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			foreach (var kvp in json.Object)
			{
				this[kvp.Key] = serializer.Deserialize<JsonSchema>(kvp.Value);
			}
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.ToDictionary(kvp => kvp.Key,
									 kvp => serializer.Serialize(kvp.Value))
				.ToJson();
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(DefsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			var definitionsMatch = this.FullOuterJoin(other,
													  tk => tk.Key,
													  ok => ok.Key,
													  (tk, ok) => new {ThisDefinition = tk.Value, OtherDefinition = ok.Value})
				.ToList();

			return definitionsMatch.All(k => Equals(k.ThisDefinition, k.OtherDefinition));

		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as DefsKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as DefsKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}