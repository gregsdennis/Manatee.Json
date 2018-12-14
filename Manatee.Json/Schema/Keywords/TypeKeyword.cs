using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the <code>type</code> JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class TypeKeyword : IJsonSchemaKeyword, IEquatable<TypeKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "type";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 1;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Assertion;

		/// <summary>
		/// The schema value type for this keyword.
		/// </summary>
		public JsonSchemaType Value { get; private set; }

		/// <summary>
		/// Used for deserialization.
		/// </summary>
		[DeserializationUseOnly]
		public TypeKeyword() { }
		/// <summary>
		/// Creates an instance of the <see cref="TypeKeyword"/>.
		/// </summary>
		public TypeKeyword(JsonSchemaType type)
		{
			Value = type;
		}

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = new SchemaValidationResults(Name, context);

			bool valid = true;
			switch (context.Instance.Type)
			{
				case JsonValueType.Number:
					if (Value.HasFlag(JsonSchemaType.Number)) break;
					if (context.Instance.Number.IsInt() && Value.HasFlag(JsonSchemaType.Integer)) break;
					valid = false;
					break;
				case JsonValueType.String:
					var expected = Value.ToJson();
					if (expected.Type == JsonValueType.String && expected == context.Instance) break;
					if (expected.Type == JsonValueType.Array && expected.Array.Contains(context.Instance)) break;
					if (Value.HasFlag(JsonSchemaType.String)) break;
					valid = false;
					break;
				case JsonValueType.Boolean:
					if (Value.HasFlag(JsonSchemaType.Boolean)) break;
					valid = false;
					break;
				case JsonValueType.Object:
					if (Value.HasFlag(JsonSchemaType.Object)) break;
					valid = false;
					break;
				case JsonValueType.Array:
					if (Value.HasFlag(JsonSchemaType.Array)) break;
					valid = false;
					break;
				case JsonValueType.Null:
					if (Value.HasFlag(JsonSchemaType.Null)) break;
					valid = false;
					break;
			}

			if (!valid)
			{
				results.IsValid = false;
				results.Keyword = Name;
				results.AdditionalInfo["expected"] = Value.ToJson();
				results.AdditionalInfo["actual"] = context.Instance.Type.ToString().ToLower();
			}

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with <code>$ref</code> keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		public void RegisterSubschemas(Uri baseUri) { }
		/// <summary>
		/// Resolves any subschemas during resolution of a <code>$ref</code> during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
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
			Value = json.ToSchemaType();
		}

		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value.ToJson();
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(TypeKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as TypeKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as TypeKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}