namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `uri-reference` format validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression used is:
	///
	/// ```
	/// ^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?
	/// ```
	/// </remarks>
	public class UriReferenceFormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new UriReferenceFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "uri-reference";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft2019_09;

		private UriReferenceFormatValidator()
			: base(@"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?") { }
	}
}