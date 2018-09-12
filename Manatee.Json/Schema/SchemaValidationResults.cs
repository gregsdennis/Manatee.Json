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
		public string ErroredKeyword { get; set; }
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

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var obj = new JsonObject();
			obj["valid"] = IsValid;
			var relativeLocation = RelativeLocation.ToString();
			obj["keywordLocation"] = relativeLocation;
			if (AbsoluteLocation != null && (AbsoluteLocation.Fragment != relativeLocation || RelativeLocation.Contains("$ref")))
				obj["absoluteKeywordLocation"] = AbsoluteLocation.OriginalString;
			obj["instanceLocation"] = InstanceLocation.ToString();

			var nonNullNestedResults = NestedResults.Where(r => !ReferenceEquals(r, Null)).ToList();
			if (IsValid)
			{
				if (AnnotationValue != null)
					obj["annotation"] = AnnotationValue;
				if (nonNullNestedResults.Any())
					obj["annotations"] = nonNullNestedResults.Select(r => r.ToJson(serializer)).ToJson();
			}
			else
			{
				// TODO: process error messages in addition to adding the keyword
				if (ErroredKeyword != null)
					obj["offendingKeyword"] = ErroredKeyword;
				if (nonNullNestedResults.Any())
					obj["errors"] = nonNullNestedResults.Select(r => r.ToJson(serializer)).ToJson();
			}
			if (AdditionalInfo.Any())
				obj["additionalInfo"] = AdditionalInfo;

			return obj;
		}
		public bool Equals(SchemaValidationResults other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return IsValid == other.IsValid &&
			       Equals(RelativeLocation, other.RelativeLocation) &&
			       Equals(AbsoluteLocation, other.AbsoluteLocation) &&
			       Equals(InstanceLocation, other.InstanceLocation) &&
			       Equals(AnnotationValue, other.AnnotationValue) &&
			       string.Equals(ErroredKeyword, other.ErroredKeyword) &&
			       Equals(AdditionalInfo, other.AdditionalInfo) &&
			       NestedResults.ContentsEqual(other.NestedResults);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as SchemaValidationResults);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = IsValid.GetHashCode();
				hashCode = (hashCode * 397) ^ (RelativeLocation?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (AbsoluteLocation?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (InstanceLocation?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (AnnotationValue?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (ErroredKeyword?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (AdditionalInfo?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ (NestedResults?.GetCollectionHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}