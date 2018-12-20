using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Contains the results of schema validation.
	/// </summary>
	public class SchemaValidationResults : IJsonSerializable, IEquatable<SchemaValidationResults>
	{
		/// <summary>
		/// Gets a value to return for no results.  Only used when a keyword should not produce any output, such as <code>$id</code>.
		/// </summary>
		public static SchemaValidationResults Null { get; } = new SchemaValidationResults();

		/// <summary>
		/// Gets or sets whether the validation was successful.  Defaults to true.
		/// </summary>
		public bool IsValid { get; set; } = true;
		/// <summary>
		/// Gets or sets the location of the keyword relative to the original schema root.  This will include any <code>$ref</code> segments.
		/// </summary>
		public JsonPointer RelativeLocation { get; set; }
		/// <summary>
		/// Gets or sets the absolute location of the keyword.  This is a direct reference after all <code>$ref</code> keywords have been resolved.
		/// </summary>
		public Uri AbsoluteLocation { get; set; }
		/// <summary>
		/// Gets or sets the location within the instance.
		/// </summary>
		public JsonPointer InstanceLocation { get; set; }
		/// <summary>
		/// Gets or sets an annotation value.
		/// </summary>
		public JsonValue AnnotationValue { get; set; }
		/// <summary>
		/// Gets or sets the keyword.
		/// </summary>
		public string Keyword { get; set; }
		/// <summary>
		/// Gets or sets any additional information regarding the validation.
		/// </summary>
		public JsonObject AdditionalInfo { get; set; } = new JsonObject();
		/// <summary>
		/// Gets or sets any results of nested schemas.
		/// </summary>
		public List<SchemaValidationResults> NestedResults { get; set; } = new List<SchemaValidationResults>();

		internal bool RecursionDetected { get; set; }

		internal SchemaValidationResults() { }
		internal SchemaValidationResults(SchemaValidationContext context)
		{
			InstanceLocation = context.InstanceLocation.Clone();
			if (context.BaseUri != null)
				AbsoluteLocation = new Uri(context.BaseUri + context.BaseRelativeLocation.ToString(), UriKind.RelativeOrAbsolute);
			RelativeLocation = context.RelativeLocation;
		}
		/// <summary>
		/// Creates a new instance of the <see cref="SchemaValidationResults"/> class.
		/// </summary>
		/// <param name="keyword">The name of the keyword producing the result.</param>
		/// <param name="context">The current validation context when the result is produced.</param>
		public SchemaValidationResults(string keyword, SchemaValidationContext context)
		{
			InstanceLocation = context.InstanceLocation.Clone();
			if (context.BaseUri != null)
				AbsoluteLocation = new Uri(context.BaseUri + context.BaseRelativeLocation.CloneAndAppend(keyword).ToString(), UriKind.RelativeOrAbsolute);
			RelativeLocation = context.RelativeLocation.CloneAndAppend(keyword);
			Keyword = keyword;
		}

		/// <summary>
		/// Creates a condensed copy of the result.
		/// </summary>
		public SchemaValidationResults Condense()
		{
			var children = NestedResults.Where(r => r.RelativeLocation != null && !r.RecursionDetected)
				.Select(r => r.Condense())
				.Where(r => (!IsValid && !r.IsValid) || (IsValid && r.AnnotationValue != null) || r.NestedResults.Any())
				.Distinct()
				.ToList();

			var copy = new SchemaValidationResults();
			copy._CopyDataFrom(this);
			copy.NestedResults = children;

			if (copy.AnnotationValue != null) return copy;

			copy.NestedResults = copy.NestedResults.Where(r => r.IsValid == copy.IsValid).ToList();

			if (copy.NestedResults.Count != 1) return copy;

			copy._CopyDataFrom(copy.NestedResults[0]);

			return copy;
		}

		/// <summary>
		/// Creates a flattened copy of the result.
		/// </summary>
		public SchemaValidationResults Flatten()
		{
			var condensed = Condense();

			var children = condensed._GetAllChildren();
			condensed.NestedResults = new List<SchemaValidationResults>();

			var results = new SchemaValidationResults
				{
					IsValid = IsValid,
					NestedResults = {condensed}
				};
			results.NestedResults.AddRange(children);

			return results;
		}

		private IEnumerable<SchemaValidationResults> _GetAllChildren()
		{
			return NestedResults.Union(NestedResults.SelectMany(r => r._GetAllChildren()));
		}

		private void _CopyDataFrom(SchemaValidationResults other)
		{
			IsValid = other.IsValid;
			RelativeLocation = other.RelativeLocation;
			AbsoluteLocation = other.AbsoluteLocation;
			InstanceLocation = other.InstanceLocation;
			AnnotationValue = other.AnnotationValue;
			Keyword = other.Keyword;
			AdditionalInfo = other.AdditionalInfo;
			NestedResults = other.NestedResults;
		}

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			IsValid = obj["valid"].Boolean;
			var relativeLocation = obj.TryGetString("keywordLocation");
			if (relativeLocation != null)
				RelativeLocation = JsonPointer.Parse(relativeLocation);
			var absoluteLocation = obj.TryGetString("absoluteKeywordLocation");
			if (absoluteLocation != null)
				AbsoluteLocation = new Uri(absoluteLocation);
			var instanceLocation = obj.TryGetString("instanceLocation");
			if (instanceLocation != null)
				InstanceLocation = JsonPointer.Parse(instanceLocation);
			Keyword = obj.TryGetString("keyword");
			AnnotationValue = obj.TryGetString("annotation");
			var nested = obj.TryGetArray("annotations") ?? obj.TryGetArray("errors");
			if (nested != null)
				NestedResults = nested.Select(serializer.Deserialize<SchemaValidationResults>).ToList();
			AdditionalInfo = obj.TryGetObject("additionalInfo");
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var obj = new JsonObject();
			obj["valid"] = IsValid;
			if (RelativeLocation != null)
			{
				var relativeLocation = RelativeLocation.ToString();
				obj["keywordLocation"] = relativeLocation;
				if (AbsoluteLocation != null && (AbsoluteLocation.Fragment != relativeLocation || RelativeLocation.Contains("$ref")))
					obj["absoluteKeywordLocation"] = AbsoluteLocation.OriginalString;
			}
			if (InstanceLocation != null)
				obj["instanceLocation"] = InstanceLocation.ToString();

			var nonNullNestedResults = NestedResults.Where(r => !ReferenceEquals(r, Null)).ToList();
			if (Keyword != null)
				obj["keyword"] = Keyword;
			if (IsValid)
			{
				if (AnnotationValue != null)
					obj["annotation"] = AnnotationValue;
				if (nonNullNestedResults.Any())
					obj["annotations"] = nonNullNestedResults.Select(r => r.ToJson(serializer)).ToJson();
			}
			else
			{
				// TODO: process error messages
				if (nonNullNestedResults.Any())
					obj["errors"] = nonNullNestedResults.Select(r => r.ToJson(serializer)).ToJson();
			}
			if (AdditionalInfo.Any())
				obj["additionalInfo"] = AdditionalInfo;

			return obj;
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(SchemaValidationResults other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return IsValid == other.IsValid &&
			       Equals(RelativeLocation, other.RelativeLocation) &&
			       Equals(AbsoluteLocation, other.AbsoluteLocation) &&
			       Equals(InstanceLocation, other.InstanceLocation) &&
			       Equals(AnnotationValue, other.AnnotationValue) &&
			       string.Equals(Keyword, other.Keyword) &&
			       Equals(AdditionalInfo, other.AdditionalInfo) &&
			       NestedResults.ContentsEqual(other.NestedResults);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as SchemaValidationResults);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = IsValid.GetHashCode();
				hashCode = (hashCode * 397) ^ (RelativeLocation?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (AbsoluteLocation?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (InstanceLocation?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (AnnotationValue?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (Keyword?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (AdditionalInfo?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (NestedResults?.GetCollectionHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}