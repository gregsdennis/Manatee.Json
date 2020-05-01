using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the `contains` JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}")]
	public class ContainsKeyword : IJsonSchemaKeyword, IEquatable<ContainsKeyword>
	{
		/// <summary>
		/// Gets or sets the error message template.
		/// </summary>
		/// <remarks>
		/// Does not supports any tokens.
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "Expected an item that matched the given schema but no such items were found.";

		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "contains";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft2019_09;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		/// <implementationNotes>
		/// May be duplicated across different keywords.  This property comes into play when there
		/// are several keywords which must be evaluated in a specific order.
		/// </implementationNotes>
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
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		[DeserializationUseOnly]
		[UsedImplicitly]
		public ContainsKeyword() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		/// <summary>
		/// Creates an instance of the <see cref="AdditionalItemsKeyword"/>.
		/// </summary>
		public ContainsKeyword(JsonSchema value)
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
			if (context.Instance.Type != JsonValueType.Array)
			{
				Log.Schema(() => "Instance not an array; not applicable");
				return new SchemaValidationResults(Name, context);
			}

			var baseRelativeLocation = context.BaseRelativeLocation?.CloneAndAppend(Name);
			var relativeLocation = context.RelativeLocation.CloneAndAppend(Name);

			var valid = false;
			var reportChildErrors = JsonSchemaOptions.ShouldReportChildErrors(this, context);
			var i = 0;
			var nestedResults = new List<SchemaValidationResults>();
			var matchedIndices = new JsonArray();
			var hasMinMaxConstraints = context.Local.Get<MinContainsKeyword>() != null ||
			                           context.Local.Get<MaxContainsKeyword>() != null;

			foreach (var jv in context.Instance.Array)
			{
				var newContext = new SchemaValidationContext(context)
					{
						Instance = jv,
						BaseRelativeLocation = baseRelativeLocation,
						RelativeLocation = relativeLocation,
						InstanceLocation = context.InstanceLocation.CloneAndAppend(i.ToString()),
					};
				var localResults = Value.Validate(newContext);
				valid |= localResults.IsValid;
				if (localResults.IsValid)
				{
					if (context.ShouldTrackValidatedValues)
						context.LocallyValidatedIndices.Add(i);

					matchedIndices.Add(i);
				}

				if (JsonSchemaOptions.OutputFormat == SchemaValidationOutputFormat.Flag)
				{
					if (valid && !hasMinMaxConstraints)
					{
						Log.Schema(() => "Match found and no min/max constraints; halting validation early");
						break;
					}
				}
				else if (reportChildErrors)
					nestedResults.Add(localResults);

				i++;
			}

			Log.Schema(() => $"Found {matchedIndices.Count} instances that match; saving for later");
			context.Misc["containsCount"] = matchedIndices.Count;
			var results = new SchemaValidationResults
				{
					NestedResults = nestedResults,
					IsValid = valid,
					Keyword = Name,
					AdditionalInfo = {["matchedIndices"] = matchedIndices}
				};
			if (!valid)
				results.ErrorMessage = ErrorTemplate;

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$ref` keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		/// <param name="localRegistry">A local schema registry to handle cases where <paramref name="baseUri"/> is null.</param>
		public void RegisterSubschemas(Uri? baseUri, JsonSchemaRegistry localRegistry)
		{
			Value.RegisterSubschemas(baseUri, localRegistry);
		}
		/// <summary>
		/// Resolves any subschemas during resolution of a `$ref` during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <param name="supportedVersions">Indicates the root schema's supported versions.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema? ResolveSubschema(JsonPointer pointer, Uri baseUri, JsonSchemaVersion supportedVersions)
		{
			return Value.ResolveSubschema(pointer, baseUri, supportedVersions);
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
		public bool Equals(ContainsKeyword? other)
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
			return Equals(other as ContainsKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object? obj)
		{
			return Equals(obj as ContainsKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}