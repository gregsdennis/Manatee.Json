using System;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the <code>minContains</code> JSON Schema keyword.
	/// </summary>
	public class MinContainsKeyword : IJsonSchemaKeyword, IEquatable<MinContainsKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "minContains";

		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions => JsonSchemaVersion.Draft08;

		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		/// <implementationNotes>
		/// May be duplicated across different keywords.  This property comes into play when there
		/// are several keywords which must be evaluated in a specific order.
		/// </implementationNotes>
		public int ValidationSequence => 2;

		/// <summary>
		/// The numeric value for this keyword.
		/// </summary>
		public uint Value { get; set; }

		/// <summary>
		/// Used for deserialization.
		/// </summary>
		[DeserializationUseOnly]
		public MinContainsKeyword() { }
		/// <summary>
		/// Creates an instance of the <see cref="MinContainsKeyword"/>.
		/// </summary>
		public MinContainsKeyword(uint value)
		{
			Value = value;
		}

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = (uint) json.Number;
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
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as MinContainsKeyword);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(MinContainsKeyword other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Value == other.Value;
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as MinContainsKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (int) Value;
		}
		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = new SchemaValidationResults(Name, context);

			if (context.Instance.Type != JsonValueType.Array) return results;
			if (!context.Misc.TryGetValue("containsCount", out var value)) return results;

			var containsCount = (int) value;

			if (containsCount < Value)
			{
				results.IsValid = false;
				results.AdditionalInfo["contains"] = containsCount;
				results.AdditionalInfo["minContains"] = Value;
			}

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
