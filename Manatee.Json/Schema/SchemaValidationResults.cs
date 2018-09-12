using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Contains the results of schema validation.
	/// </summary>
	public class SchemaValidationResults : IJsonSerializable
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
		public bool InvertNestedResults { get; set; }

		public List<SchemaValidationResults> NestedResults { get; set; } = new List<SchemaValidationResults>();

		private SchemaValidationResults() { }
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

			var nonNullNestedResults = NestedResults.Where(r => r != Null).ToList();
			if (AnnotationValue != null)
			{
				obj["annotation"] = AnnotationValue;
				if (NestedResults.Any(r => r != Null))
					obj["annotations"] = serializer.Serialize(nonNullNestedResults);
			}
			else if (ErroredKeyword != null)
			{
				// TODO: process error messages in addition to adding the keyword
				obj["offendingKeyword"] = ErroredKeyword;
				if (NestedResults.Any(r => r != Null))
					obj["errors"] = serializer.Serialize(nonNullNestedResults);
			}
			if (AdditionalInfo.Any())
				obj["additionalInfo"] = AdditionalInfo;

			return obj;
		}
	}
}