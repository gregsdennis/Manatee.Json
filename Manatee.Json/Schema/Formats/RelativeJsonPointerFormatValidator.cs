namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `relative-json-pointer` format validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression used is:
	///
	/// ```
	/// ^[0-9]+#/(([^/~])|(~[01]))*$
	/// ```
	/// </remarks>
	public class RelativeJsonPointerFormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new RelativeJsonPointerFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "relative-json-pointer";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.Draft2019_09;

		private RelativeJsonPointerFormatValidator()
			: base(@"^[0-9]+#/(([^/~])|(~[01]))*$") { }
	}
}