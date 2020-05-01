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
	/// Defines the `unevaluatedProperties` JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}")]
	public class UnevaluatedPropertiesKeyword : IJsonSchemaKeyword, IEquatable<UnevaluatedPropertiesKeyword>, IRequireAnnotations
	{
		/// <summary>
		/// Gets or sets the error message template.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - properties
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "Properties {{properties}} were not covered by `properties`, `patternProperties`, " +
		                                                   "and `additionalProperties` failed validation of the local subschema.";

		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "unevaluatedProperties";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft2019_09;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => int.MaxValue;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Validation;

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
		public UnevaluatedPropertiesKeyword() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		/// <summary>
		/// Creates an instance of the <see cref="UnevaluatedPropertiesKeyword"/>.
		/// </summary>
		public UnevaluatedPropertiesKeyword(JsonSchema value)
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
			if (context.Instance.Type != JsonValueType.Object)
			{
				Log.Schema(() => "Instance not an object; not applicable");
				return new SchemaValidationResults(Name, context);
			}

			var obj = context.Instance.Object;
			var results = new SchemaValidationResults(Name, context);
			var toEvaluate = obj.Where(kvp => !context.EvaluatedPropertyNames.Contains(kvp.Key))!.ToJson();
			if (toEvaluate.Count == 0)
			{
				Log.Schema(() => "All properties have been evaluated");
				return results;
			}

			Log.Schema(() => context.EvaluatedPropertyNames.Count == 0
				            ? "No properties have been evaluated; process all"
				            : $"Properties {context.EvaluatedPropertyNames.ToJson()} have been evaluated; skipping these");
			if (Value == JsonSchema.False && toEvaluate.Any())
			{
				Log.Schema(() => "Subschema is `false`; all instances invalid");
				results.IsValid = false;
				results.Keyword = Name;
				results.AdditionalInfo["properties"] = toEvaluate.Keys.ToJson();
				results.ErrorMessage = ErrorTemplate.ResolveTokens(results.AdditionalInfo);
				return results;
			}

			var valid = true;
			var reportChildErrors = JsonSchemaOptions.ShouldReportChildErrors(this, context);
			var nestedResults = new List<SchemaValidationResults>();
			var failedProperties = new JsonArray();

			foreach (var kvp in toEvaluate)
			{
				var newContext = new SchemaValidationContext(context)
					{
						Instance = kvp.Value,
						BaseRelativeLocation = context.BaseRelativeLocation?.CloneAndAppend(Name),
						RelativeLocation = context.RelativeLocation.CloneAndAppend(Name),
						InstanceLocation = context.InstanceLocation.CloneAndAppend(kvp.Key),
					};
				var localResults = Value.Validate(newContext);
				if (!localResults.IsValid)
				{
					failedProperties.Add(kvp.Key);
				}
				valid &= localResults.IsValid;

				if (JsonSchemaOptions.OutputFormat == SchemaValidationOutputFormat.Flag)
				{
					if (!valid)
					{
						Log.Schema(() => "Subschema failed; halting validation early");
						break;
					}
				}
				else if (reportChildErrors)
					nestedResults.Add(localResults);
			}

			results.NestedResults = nestedResults;

			if (nestedResults.Any(r => !r.IsValid))
			{
				results.IsValid = false;
				results.Keyword = Name;
				results.AdditionalInfo["properties"] = failedProperties;
				results.ErrorMessage = ErrorTemplate.ResolveTokens(results.AdditionalInfo);
			}

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
			return Value.ToJson(serializer);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
		public bool Equals(UnevaluatedPropertiesKeyword? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
		public bool Equals(IJsonSchemaKeyword? other)
		{
			return Equals(other as UnevaluatedPropertiesKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object? obj)
		{
			return Equals(obj as UnevaluatedPropertiesKeyword);
		}
		/// <summary>Serves as the default hash function.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}