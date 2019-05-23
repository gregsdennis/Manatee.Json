using System;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the <code>if</code> JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}")]
	public class IfKeyword : IJsonSchemaKeyword, IEquatable<IfKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "if";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft2019_04;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 1;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Applicator;

		/// <summary>
		/// The schema value for this keyword.
		/// </summary>
		public JsonSchema Value { get; private set; }

		/// <summary>
		/// Used for deserialization.
		/// </summary>
		[DeserializationUseOnly]
		public IfKeyword() { }
		/// <summary>
		/// Creates an instance of the <see cref="IfKeyword"/>.
		/// </summary>
		public IfKeyword(JsonSchema value)
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
			var then = context.Local.Get<ThenKeyword>();
			var @else = context.Local.Get<ElseKeyword>();

			if (then == null && @else == null) return SchemaValidationResults.Null;

			var newContext = new SchemaValidationContext
				{
					BaseUri = context.BaseUri,
					Instance = context.Instance,
					Root = context.Root,
					RecursiveAnchor = context.RecursiveAnchor,
					BaseRelativeLocation = context.BaseRelativeLocation.CloneAndAppend(Name),
					RelativeLocation = context.RelativeLocation.CloneAndAppend(Name),
					InstanceLocation = context.InstanceLocation
				};

			var ifResults = Value.Validate(newContext);

			if (ifResults.IsValid)
			{
				if (then == null) return SchemaValidationResults.Null;

				var results = new SchemaValidationResults(Name, context);

				newContext = new SchemaValidationContext
					{
						BaseUri = context.BaseUri,
						Instance = context.Instance,
						Root = context.Root,
						RecursiveAnchor = context.RecursiveAnchor,
						BaseRelativeLocation = context.BaseRelativeLocation.CloneAndAppend(then.Name),
						RelativeLocation = context.RelativeLocation.CloneAndAppend(then.Name),
						InstanceLocation = context.InstanceLocation
					};
				var thenResults = then.Value.Validate(newContext);
				if (!thenResults.IsValid)
				{
					results.IsValid = false;
					results.Keyword = then.Name;
					results.NestedResults.Add(thenResults);
				}

				return results;
			}
			else
			{
				if (@else == null) return SchemaValidationResults.Null;

				var results = new SchemaValidationResults(Name, context);

				newContext = new SchemaValidationContext
					{
						BaseUri = context.BaseUri,
						Instance = context.Instance,
						Root = context.Root,
						RecursiveAnchor = context.RecursiveAnchor,
						BaseRelativeLocation = context.BaseRelativeLocation.CloneAndAppend(@else.Name),
						RelativeLocation = context.RelativeLocation.CloneAndAppend(@else.Name),
						InstanceLocation = context.InstanceLocation
					};
				var elseResults = @else.Value.Validate(newContext);
				if (!elseResults.IsValid)
				{
					results.IsValid = false;
					results.Keyword = @else.Name;
					results.NestedResults.Add(elseResults);
				}

				return results;
			}
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
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IfKeyword other)
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
			return Equals(other as IfKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as IfKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}
