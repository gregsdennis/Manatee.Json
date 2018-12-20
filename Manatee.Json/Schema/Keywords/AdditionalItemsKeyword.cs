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
	/// Defines the <code>additionalItems</code> JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}")]
	public class AdditionalItemsKeyword : IJsonSchemaKeyword, IEquatable<AdditionalItemsKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "additionalItems";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 2;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Assertion;

		/// <summary>
		/// The schema value for this keyword.
		/// </summary>
		public JsonSchema Value { get; private set; }

		/// <summary>
		/// Used for deserialization.
		/// </summary>
		[DeserializationUseOnly]
		public AdditionalItemsKeyword() { }
		/// <summary>
		/// Creates an instance of the <see cref="AdditionalItemsKeyword"/>.
		/// </summary>
		public AdditionalItemsKeyword(JsonSchema value)
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
			if (context.Instance.Type != JsonValueType.Array) return new SchemaValidationResults(Name, context);

			var itemsKeyword = context.Local.Get<ItemsKeyword>();
			if (itemsKeyword == null || !itemsKeyword.IsArray) return new SchemaValidationResults(Name, context);

			var nestedResults = new List<SchemaValidationResults>();
			var array = context.Instance.Array;

			if (context.LocalTierLastEvaluatedIndex < array.Count)
			{
				nestedResults.AddRange(array.Skip(context.LocalTierLastEvaluatedIndex).Select((jv, i) =>
					{
						var baseRelativeLocation = context.BaseRelativeLocation.CloneAndAppend(Name);
						var relativeLocation = context.RelativeLocation.CloneAndAppend(Name);
						var newContext = new SchemaValidationContext
							{
								BaseUri = context.BaseUri,
								Instance = jv,
								Root = context.Root,
								RecursiveAnchor = context.RecursiveAnchor,
								BaseRelativeLocation = baseRelativeLocation,
								RelativeLocation = relativeLocation,
								InstanceLocation = context.InstanceLocation.CloneAndAppend(i.ToString())
							};
						var localResults = Value.Validate(newContext);
						context.LastEvaluatedIndex = Math.Max(context.LastEvaluatedIndex, i);
						context.LocalTierLastEvaluatedIndex = Math.Max(context.LastEvaluatedIndex, i);
						return localResults;
					}));
			}

			var results = new SchemaValidationResults(Name, context) {NestedResults = nestedResults};

			if (nestedResults.Any(r => !r.IsValid))
			{
				results.IsValid = false;
				results.Keyword = Name;
			}

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with <code>$ref</code> keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		public void RegisterSubschemas(Uri baseUri)
		{
			Value.RegisterSubschemas(baseUri);
		}
		/// <summary>
		/// Resolves any subschemas during resolution of a <code>$ref</code> during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return Value.ResolveSubschema(pointer, baseUri);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return serializer.Serialize(Value);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
		public bool Equals(AdditionalItemsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as AdditionalItemsKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as AdditionalItemsKeyword);
		}
		/// <summary>Serves as the default hash function.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}