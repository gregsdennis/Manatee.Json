using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Results object for schema meta-validations.
	/// </summary>
	public class MetaSchemaValidationResults
	{
		/// <summary>
		/// Gets or sets the JSON Schema draft versions supported by this schema.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; set; }
		/// <summary>
		/// Gets a set of results produced by validating this schema against the draft meta-schemas.
		/// </summary>
		public Dictionary<string, SchemaValidationResults> MetaSchemaValidations { get; } = new Dictionary<string, SchemaValidationResults>();
		/// <summary>
		/// Gets other errors that may have been found.
		/// </summary>
		public List<string> OtherErrors { get; } = new List<string>();

		/// <summary>
		/// Gets whether this schema is valid according to the drafts.
		/// </summary>
		public bool IsValid => MetaSchemaValidations.All(v => v.Value.IsValid) &&
		                       !OtherErrors.Any();
	}
}