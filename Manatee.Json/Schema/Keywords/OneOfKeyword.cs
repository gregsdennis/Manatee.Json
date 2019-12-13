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
	/// Defines the `oneOf` JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class OneOfKeyword : List<JsonSchema>, IJsonSchemaKeyword, IEquatable<OneOfKeyword>
	{
		/// <summary>
		/// Gets or sets the error message template.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - passed
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "Expected exactly one subschema to pass validation, but found {{passed}}.";

		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "oneOf";
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
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Applicator;

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var reportChildErrors = JsonSchemaOptions.ShouldReportChildErrors(this, context);
			var i = 0;
			var nestedResults = new List<SchemaValidationResults>();
			var validCount = 0;

			var contextCopy = new SchemaValidationContext(context);

			foreach (var s in this)
			{
				var newContext = new SchemaValidationContext(context)
					{
						BaseRelativeLocation = context.BaseRelativeLocation?.CloneAndAppend(Name, i.ToString()),
						RelativeLocation = context.RelativeLocation.CloneAndAppend(Name, i.ToString()),
					};
				var localResults = s.Validate(newContext);
				if (localResults.IsValid)
					validCount++;
				JsonOptions.Log?.Verbose($"`{Name}` {validCount} items valid so far");
				contextCopy.UpdateEvaluatedPropertiesAndItemsFromSubschemaValidation(newContext);

				if (JsonSchemaOptions.OutputFormat == SchemaValidationOutputFormat.Flag)
				{
					if (validCount > 1)
					{
						JsonOptions.Log?.Verbose("More than one subschema succeeded; halting validation early");
						break;
					}
				}
				else if (reportChildErrors)
					nestedResults.Add(localResults);
			}

			var results = new SchemaValidationResults(Name, context)
				{
					IsValid = validCount == 1,
					NestedResults = nestedResults
				};

			if (!results.IsValid)
			{
				JsonOptions.Log?.Verbose($"{validCount} subschemas passed validation; expected only one");
				results.AdditionalInfo["passed"] = validCount;
				results.ErrorMessage = ErrorTemplate.ResolveTokens(results.AdditionalInfo);
			}
			else
			{
				context.UpdateEvaluatedPropertiesAndItemsFromSubschemaValidation(contextCopy);
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

			if (!int.TryParse(first, out var index) || index < 0 || index >= Count) return null;

			return this[index].ResolveSubschema(new JsonPointer(pointer.Skip(1)), baseUri);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(serializer.Deserialize<JsonSchema>));
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var array = this.Select(serializer.Serialize).ToJson();
			array.EqualityStandard = ArrayEquality.ContentsEqual;

			return array;
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(OneOfKeyword other)
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
			return Equals(other as OneOfKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as OneOfKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}