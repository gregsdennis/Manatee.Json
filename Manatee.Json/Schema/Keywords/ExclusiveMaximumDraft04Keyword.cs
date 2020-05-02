using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the `exclusiveMaximum` JSON Schema keyword for draft-04 schemas.
	/// </summary>
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class ExclusiveMaximumDraft04Keyword : IJsonSchemaKeywordPlus, IEquatable<ExclusiveMaximumDraft04Keyword>
	{
		/// <summary>
		/// Gets or sets the error message template.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - actual
		/// - upperBound
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "The value {{actual}} should be strictly less than {{upperBound}}.";

		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "exclusiveMaximum";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 1;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.None;

		/// <summary>
		/// The boolean value for this keyword.
		/// </summary>
		public bool Value { get; private set; }

		/// <summary>
		/// Used for deserialization.
		/// </summary>
		[DeserializationUseOnly]
		[UsedImplicitly]
		public ExclusiveMaximumDraft04Keyword() { }
		/// <summary>
		/// Creates an instance of the <see cref="ExclusiveMaximumDraft04Keyword"/>.
		/// </summary>
		public ExclusiveMaximumDraft04Keyword(bool value)
		{
			Value = value;
		}

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = new SchemaValidationResults(Name, context);

			if (context.Instance.Type != JsonValueType.Number)
			{
				Log.Schema(() => "Instance not a number; not applicable");
				return results;
			}

			var keyword = context.Local.Get<MaximumKeyword>();
			if (keyword == null)
			{
				Log.Schema(() => "`maximum` keyword not defined; not applicable");
				return results;
			}

			if (!Value)
			{
				Log.Schema(() => "Not exclusive; see `maximum` results");
				return results;
			}
			
			if (context.Instance.Number >= keyword.Value)
			{
				Log.Schema(() => $"Bounds check failed: {context.Instance.Number} >= {keyword.Value}");
				results.IsValid = false;
				results.AdditionalInfo["upperBound"] = keyword.Value;
				results.AdditionalInfo["actual"] = context.Instance;
				results.ErrorMessage = ErrorTemplate.ResolveTokens(results.AdditionalInfo);
			}

			return results;
		}
		bool IJsonSchemaKeywordPlus.Handles(JsonValue value)
		{
			return value.Type == JsonValueType.Boolean;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$ref` keywords.
		/// </summary>
		/// <param name="context">The context object.</param>
		public void RegisterSubschemas(SchemaValidationContext context) { }
		/// <summary>
		/// Resolves any subschemas during resolution of a `$ref` during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <param name="supportedVersions">Indicates the root schema's supported versions.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema? ResolveSubschema(JsonPointer pointer, Uri baseUri, JsonSchemaVersion supportedVersions)
		{
			return null;
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Boolean;
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ExclusiveMaximumDraft04Keyword? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword? other)
		{
			return Equals(other as ExclusiveMaximumDraft04Keyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object? obj)
		{
			return Equals(obj as ExclusiveMaximumDraft04Keyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (Value.GetHashCode());
		}
	}
}