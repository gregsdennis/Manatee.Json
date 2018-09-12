using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Pointer;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Contains the results of schema validation.
	/// </summary>
	public class SchemaValidationResults 
	{
		public static SchemaValidationResults Null { get; } = new SchemaValidationResults();

		/// <summary>
		/// Gets whether the validation was successful.
		/// </summary>
		public bool IsValid => ErroredKeyword == null && NestedResults.All(r => r.IsValid);

		public JsonPointer RelativeLocation { get; set; }
		public Uri AbsoluteLocation { get; set; }
		public JsonPointer InstanceLocation { get; set; }
		public JsonValue AnnotationValue { get; set; }
		public string ErroredKeyword { get; set; }
		public JsonObject AdditionalInfo { get; set; } = new JsonObject();

		public List<SchemaValidationResults> NestedResults { get; set; } = new List<SchemaValidationResults>();

		private SchemaValidationResults() { }
		internal SchemaValidationResults(SchemaValidationContext context)
		{
			InstanceLocation = context.InstanceLocation.Clone();
			if (context.BaseUri != null)
				AbsoluteLocation = new Uri(context.BaseUri, context.BaseRelativeLocation.ToString());
			RelativeLocation = context.RelativeLocation;
		}
		public SchemaValidationResults(string keyword, SchemaValidationContext context)
		{
			InstanceLocation = context.InstanceLocation.Clone();
			if (context.BaseUri != null)
				AbsoluteLocation = new Uri(context.BaseUri, context.BaseRelativeLocation.CloneAndAppend(keyword).ToString());
			RelativeLocation = context.RelativeLocation.CloneAndAppend(keyword);
		}
	}
}