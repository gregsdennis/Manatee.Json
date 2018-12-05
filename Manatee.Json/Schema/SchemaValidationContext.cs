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
		/// Gets or sets the recursive root.
		/// </summary>
		public Stack<JsonSchema> RecursiveRoot { get; } = new Stack<JsonSchema>();
		/// <summary>
		/// Gets or sets the instance being validated.
		/// </summary>
		public JsonValue Instance { get; set; }
		/// <summary>
		/// Gets a list of property names that have been evaluated in this validation pass.
		/// </summary>
		public List<string> EvaluatedPropertyNames { get; } = new List<string>();
		/// <summary>
		/// Gets a list of property names that have been evaluated on the current tier of this validation pass.
		/// </summary>
		public List<string> LocallyEvaluatedPropertyNames { get; } = new List<string>();
		/// <summary>
		/// Gets the last array index that has been evaluated in this validation pass.
		/// </summary>
		public int LastEvaluatedIndex { get; set; }
		/// <summary>
		/// Gets the last array index that has been evaluated on the current tier of this validation pass.
		/// </summary>
		public int LocalTierLastEvaluatedIndex { get; set; }
		/// <summary>
		/// Gets or sets the base URI at this point in the validation.
		/// </summary>
		public Uri BaseUri { get; set; }
		/// <summary>
		/// Gets or sets the current instance location.
		/// </summary>
		public JsonPointer InstanceLocation { get; set; }
		/// <summary>
		/// Gets or sets the current schema keyword location relative to the original schema root.
		/// </summary>
		public JsonPointer RelativeLocation { get; set; }
		/// <summary>
		/// Gets or sets the current schema location relative to the current base URI (<see cref="BaseUri"/>).
		/// </summary>
		public JsonPointer BaseRelativeLocation { get; set; }
		/// <summary>
		/// Miscellaneous data.  Useful for communicating results between keywords.
		/// </summary>
		/// <remarks>
		/// Use <see cref="IJsonSchemaKeyword.ValidationSequence"/> to ensure that keywords are
		/// processed in the correct order so that the communication occurs properly.
		/// </remarks>
		public Dictionary<string, object> Misc { get; } = new Dictionary<string, object>();
	}
}