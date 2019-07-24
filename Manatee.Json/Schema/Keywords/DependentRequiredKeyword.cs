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
	/// Defines the `dependentRequired` JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class DependentRequiredKeyword : List<PropertyDependency>, IJsonSchemaKeyword, IEquatable<DependentRequiredKeyword>
	{
		/// <summary>
		/// Gets or sets the error message template.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - failed
		/// - total
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "{{failed}} of {{total}} dependencies failed validation.";

		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "dependentRequired";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions => JsonSchemaVersion.Draft2019_06;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 1;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Validation;

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var baseRelativeLocation = context.BaseRelativeLocation.CloneAndAppend(Name);
			var relativeLocation = context.RelativeLocation.CloneAndAppend(Name);

			var valid = true;
			var reportChildErrors = JsonSchemaOptions.ShouldReportChildErrors(this, context);
			var nestedResults = new List<SchemaValidationResults>();
			var failedCount = 0;

			foreach (var d in this)
			{
				var newContext = new SchemaValidationContext(context)
					{
						BaseRelativeLocation = baseRelativeLocation,
						RelativeLocation = relativeLocation,
						Misc = {["dependencyParent"] = Name}
					};
				var localResults = d.Validate(newContext);
				valid &= localResults.IsValid;
				if (!valid)
					failedCount++;

				if (JsonSchemaOptions.OutputFormat == SchemaValidationOutputFormat.Flag)
				{
					if (!valid) break;
				}
				else if (reportChildErrors)
					nestedResults.Add(localResults);
			}

			var results = new SchemaValidationResults(Name, context)
				{
					IsValid = valid
				};
			if (JsonSchemaOptions.OutputFormat == SchemaValidationOutputFormat.Flag)
				results.NestedResults = nestedResults;
			else if (!results.IsValid)
			{
				results.AdditionalInfo["failed"] = failedCount;
				results.AdditionalInfo["total"] = Count;
				results.ErrorMessage = ErrorTemplate.ResolveTokens(results.AdditionalInfo);
			}

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$ref` keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		/// <param name="localRegistry">A local schema registry to handle cases where <paramref name="baseUri"/> is null.</param>
		public void RegisterSubschemas(Uri baseUri, JsonSchemaRegistry localRegistry)
		{
			foreach (var schema in this)
			{
				schema.RegisterSubschemas(baseUri, localRegistry);
			}
		}
		/// <summary>
		/// Resolves any subschemas during resolution of a `$ref` during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			var first = pointer.FirstOrDefault();
			if (first == null) return null;

			var keyword = this.FirstOrDefault(k => k.PropertyName == first);

			return keyword?.ResolveSubschema(new JsonPointer(pointer.Skip(1)), baseUri);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Object.Select(kvp => new PropertyDependency(kvp.Key, kvp.Value.Array.Select(jv => jv.String))));
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.ToDictionary(d => d.PropertyName,
									 d => d.ToJson(serializer))
					   .ToJson();
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(DependentRequiredKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as DependentRequiredKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as DependentRequiredKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}