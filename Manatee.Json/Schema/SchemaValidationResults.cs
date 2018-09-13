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
		public static SchemaValidationResults Null { get; } = new SchemaValidationResults();

		/// <summary>
		/// Gets whether the validation was successful.  Defaults to true.
		/// </summary>
		public bool IsValid { get; set; } = true;
		public JsonPointer RelativeLocation { get; set; }
		public Uri AbsoluteLocation { get; set; }
		public JsonPointer InstanceLocation { get; set; }
		public JsonValue AnnotationValue { get; set; }
		// TODO: Set this for annotation-generating keywords, too
		public string Keyword { get; set; }
		public JsonObject AdditionalInfo { get; set; } = new JsonObject();

		public List<SchemaValidationResults> NestedResults { get; set; } = new List<SchemaValidationResults>();

		internal SchemaValidationResults() { }
		internal SchemaValidationResults(SchemaValidationContext context)
		{
			InstanceLocation = context.InstanceLocation.Clone();
			if (context.BaseUri != null)
				AbsoluteLocation = new Uri(context.BaseUri + context.BaseRelativeLocation.ToString(), UriKind.RelativeOrAbsolute);
			RelativeLocation = context.RelativeLocation;
		}
		public SchemaValidationResults(string keyword, SchemaValidationContext context)
		{
			InstanceLocation = context.InstanceLocation.Clone();
			if (context.BaseUri != null)
				AbsoluteLocation = new Uri(context.BaseUri + context.BaseRelativeLocation.ToString(), UriKind.RelativeOrAbsolute);
			RelativeLocation = context.RelativeLocation.CloneAndAppend(keyword);
		}

		// TODO: clone and return
		public SchemaValidationResults Condense()
		{
			var children = NestedResults.Select(r => r.Condense()).ToList();

			var copy = new SchemaValidationResults();
			copy._CopyDataFrom(this);
			copy.NestedResults = children;

			if (copy.AnnotationValue != null) return copy;
			if (copy.NestedResults.Count != 1) return copy;

			copy._CopyDataFrom(copy.NestedResults[0]);

			return copy;
		}

		// TODO: clone and return
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
			throw new NotImplementedException();
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