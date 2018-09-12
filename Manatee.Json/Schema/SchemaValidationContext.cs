using System;
using System.Collections.Generic;
using Manatee.Json.Pointer;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Used to track data throughout the validation process.
	/// </summary>
	public class SchemaValidationContext
	{
		/// <summary>
		/// Gets or sets the local schema at this point in the validation.
		/// </summary>
		public JsonSchema Local { get; set; }
		/// <summary>
		/// Gets or sets the root schema when validation begins.
		/// </summary>
		public JsonSchema Root { get; set; }
		/// <summary>
		/// Gets or sets the instance being validated.
		/// </summary>
		public JsonValue Instance { get; set; }
		/// <summary>
		/// Gets a list of property names that have been evaluated in this validation pass.
		/// </summary>
		public List<string> EvaluatedPropertyNames { get; } = new List<string>();
		/// <summary>
		/// Gets or sets the base URI at this point in the validation.
		/// </summary>
		public Uri BaseUri { get; set; }
		public JsonPointer InstanceLocation { get; set; }
		public JsonPointer RelativeLocation { get; set; }
		public JsonPointer BaseRelativeLocation { get; set; }
	}
}