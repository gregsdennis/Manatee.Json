namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `uuid` format validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression used is:
	///
	/// ```
	/// ^[0-9a-fA-F]{8}\-?[0-9a-fA-F]{4}\-?[0-9a-fA-F]{4}\-?[0-9a-fA-F]{4}\-?[0-9a-fA-F]{12}$
	/// ```
	/// </remarks>
	public class UuidReferenceFormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new UuidReferenceFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "uuid";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft2019_09;

		private UuidReferenceFormatValidator()
			: base(@"^[0-9a-fA-F]{8}\-?[0-9a-fA-F]{4}\-?[0-9a-fA-F]{4}\-?[0-9a-fA-F]{4}\-?[0-9a-fA-F]{12}$") { }
	}
}