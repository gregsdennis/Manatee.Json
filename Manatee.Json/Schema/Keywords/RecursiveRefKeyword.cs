using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the `$recursiveRef` JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name} Value={Reference}")]
	public class RecursiveRefKeyword : IJsonSchemaKeyword, IEquatable<RecursiveRefKeyword>
	{
		private readonly List<JsonPointer> _validatingLocations = new List<JsonPointer>();
		private JsonSchema? _resolvedRoot;
		private JsonPointer? _resolvedFragment;

		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "$recursiveRef";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft2019_09;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 0;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Core;

		/// <summary>
		/// Gets the reference value for this keyword.
		/// </summary>
		public string Reference { get; private set; }

		/// <summary>
		/// Gets the resolved schema that corresponds to the reference.
		/// </summary>
		public JsonSchema? Resolved { get; private set; }

		/// <summary>
		/// Used for deserialization.
		/// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		[DeserializationUseOnly]
		[UsedImplicitly]
		public RecursiveRefKeyword() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		/// <summary>
		/// Creates an instance of the <see cref="RecursiveRefKeyword"/>.
		/// </summary>
		public RecursiveRefKeyword(string reference)
		{
			Reference = reference;
		}

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (_validatingLocations.Any(l => Equals(l, context.InstanceLocation)))
				return new SchemaValidationResults(Name, context)
					{
						RecursionDetected = true,
						AnnotationValue = "Detected recursive loop. Processing halted on this branch."
					};

			if (Resolved == null)
			{
				_ResolveReference(context);
				if (Resolved == null)
					throw new SchemaReferenceNotFoundException(context.RelativeLocation);

				Log.Schema("Reference found");
			}

			var results = new SchemaValidationResults(Name, context);

			var newContext = new SchemaValidationContext(context)
				{
					BaseUri = _resolvedRoot!.DocumentPath,
					Root = _resolvedRoot ?? context.Root,
					BaseRelativeLocation = _resolvedFragment!.WithHash(),
					RelativeLocation = context.RelativeLocation.CloneAndAppend(Name),
				};

			_validatingLocations.Add(context.InstanceLocation);
			var nestedResults = Resolved.Validate(newContext);
			_validatingLocations.Remove(context.InstanceLocation);

			results.IsValid = nestedResults.IsValid;
			results.NestedResults.Add(nestedResults);
			context.UpdateEvaluatedPropertiesAndItemsFromSubschemaValidation(newContext);

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$recursiveRef` keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		/// <param name="localRegistry"></param>
		public void RegisterSubschemas(Uri? baseUri, JsonSchemaRegistry localRegistry) { }
		/// <summary>
		/// Resolves any subschemas during resolution of a `$recursiveRef` during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema? ResolveSubschema(JsonPointer pointer, Uri baseUri)
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
			Reference = json.String;
		}

		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Reference;
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(RecursiveRefKeyword? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Reference, other.Reference);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword? other)
		{
			return Equals(other as RecursiveRefKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object? obj)
		{
			return Equals(obj as RecursiveRefKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return Reference?.GetHashCode() ?? 0;
		}

		private void _ResolveReference(SchemaValidationContext context)
		{
			Log.Schema($"Resolving `{Reference}`");
	
			if (context.RecursiveAnchor != null)
			{
				Log.Schema("Finding anchor of root schema");
				if (context.BaseUri == null)
					throw new InvalidOperationException("BaseUri not set");
				var baseDocument = JsonSchemaRegistry.Get(context.BaseUri.OriginalString);
				if (baseDocument?.Get<RecursiveAnchorKeyword>() != null)
					_resolvedRoot = context.RecursiveAnchor;
			}

			if (Reference.IsLocalSchemaId())
			{
				Log.Schema("Reference recognized as anchor or local ID");
				Resolved = context.LocalRegistry.GetLocal(Reference);
				if (Resolved != null) return;

				Log.Schema($"`{Reference}` is an unknown anchor");
			}

			var documentPath = _resolvedRoot?.DocumentPath ?? context.BaseUri;
			var referenceParts = Reference.Split(new[] {'#'}, StringSplitOptions.None);
			var address = string.IsNullOrWhiteSpace(referenceParts[0]) ? documentPath?.OriginalString : referenceParts[0];
			_resolvedFragment = referenceParts.Length > 1 ? JsonPointer.Parse(referenceParts[1]) : new JsonPointer();
			if (_resolvedRoot == null)
			{
				if (!string.IsNullOrWhiteSpace(address))
				{
					if (!Uri.TryCreate(address, UriKind.Absolute, out _))
						address = context.Local.Id + address;

					if (documentPath != null && !Uri.TryCreate(address, UriKind.Absolute, out _))
					{
						var uriFolder = documentPath.OriginalString.EndsWith("/") ? documentPath : documentPath.GetParentUri();
						var absolute = new Uri(uriFolder, address);
						address = absolute.OriginalString;
					}

					_resolvedRoot = JsonSchemaRegistry.Get(address);
				}
				else
					_resolvedRoot = context.Root;
			}

			if (_resolvedRoot == null)
			{
				Log.Schema("Could not resolve root of reference");
				return;
			}

			var wellKnown = JsonSchemaRegistry.GetWellKnown(Reference);
			if (wellKnown != null)
			{
				Log.Schema("Well known reference found");
				Resolved = wellKnown;
				return;
			}

			_ResolveLocalReference(_resolvedRoot?.DocumentPath ?? context.BaseUri!);
		}
		private void _ResolveLocalReference(Uri baseUri)
		{
			if (!_resolvedFragment.Any())
			{
				Resolved = _resolvedRoot;
				return;
			}

			Log.Schema($"Resolving local reference {_resolvedFragment}");
			Resolved = _resolvedRoot!.ResolveSubschema(_resolvedFragment!, baseUri);
		}
	}
}