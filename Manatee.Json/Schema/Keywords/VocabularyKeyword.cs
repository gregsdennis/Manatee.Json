using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the <code>$vocabulary</code> JSON Schema keyword.
	/// </summary>
	public class VocabularyKeyword : Dictionary<SchemaVocabulary, bool>, IJsonSchemaKeyword, IEquatable<VocabularyKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "$vocabulary";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions => JsonSchemaVersion.Draft2019_04;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		/// <implementationNotes>
		/// May be duplicated across different keywords.  This property comes into play when there
		/// are several keywords which must be evaluated in a specific order.
		/// </implementationNotes>
		public int ValidationSequence => 0;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Core;

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
				var vocabulary = SchemaKeywordCatalog.GetVocabulary(kvp.Key) ?? new SchemaVocabulary(kvp.Key);
				this[vocabulary] = kvp.Value.Boolean;
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
			var json = new JsonObject();
			foreach (var kvp in this)
			{
				json[kvp.Key.Id] = kvp.Value;
			}

			return json;
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as VocabularyKeyword);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(VocabularyKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			return other.ContentsEqual(this);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as VocabularyKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var nestedResults = new List<SchemaValidationResults>();

			foreach (var kvp in this)
			{
				var vocabulary = kvp.Key;
				var required = kvp.Value;
				if (vocabulary.MetaSchemaId != null)
				{
					var newContext = new SchemaValidationContext
						{
							BaseUri = context.BaseUri,
							Instance = context.Instance,
							Root = context.Root,
							RecursiveAnchor = context.RecursiveAnchor,
							BaseRelativeLocation = context.BaseRelativeLocation.CloneAndAppend(Name, vocabulary.Id),
							RelativeLocation = context.RelativeLocation.CloneAndAppend(Name, vocabulary.Id),
							InstanceLocation = context.InstanceLocation
						};
					var metaSchema = JsonSchemaRegistry.Get(vocabulary.MetaSchemaId);
					if (metaSchema != null)
						metaSchema.Validate(newContext);
					else if (required)
						nestedResults.Add(new SchemaValidationResults(Name, newContext));
				}
			}

			var results = new SchemaValidationResults(Name, context)
				{
					NestedResults = nestedResults,
					IsValid = nestedResults.All(r => r.IsValid)
				};

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with <code>$ref</code> keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		/// <implementationNotes>
		/// If the keyword does not contain any schemas (e.g. <code>maximum</code>), this method is a no-op.
		/// </implementationNotes>
		public void RegisterSubschemas(Uri baseUri) { }
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
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return null;
		}
	}
}
