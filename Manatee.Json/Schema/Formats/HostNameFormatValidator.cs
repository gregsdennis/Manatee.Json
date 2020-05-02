namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `hostname` format validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression used is:
	///
	/// ```
	/// ^(?!.{255,})([a-zA-Z0-9-]{0,63}\.)*([a-zA-Z0-9-]{0,63})$
	/// ```
	/// </remarks>
	public class HostNameFormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new HostNameFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "hostname";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.All;

		private HostNameFormatValidator()
			: base(@"^(?!.{255,})([a-zA-Z0-9-]{0,63}\.)*([a-zA-Z0-9-]{0,63})$") { }
	}
	/// <summary>
	/// Handles `ipv4` format validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression used is:
	///
	/// ```
	/// ^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$
	/// ```
	/// </remarks>
	public class Ipv4FormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new Ipv4FormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "ipv4";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.All;

		private Ipv4FormatValidator()
			: base(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$") { }
	}
	/// <summary>
	/// Handles `ipv6` format validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression used is:
	///
	/// ```
	/// ^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$
	/// ```
	/// </remarks>
	public class Ipv6FormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new Ipv6FormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "ipv6";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.All;

		private Ipv6FormatValidator()
			: base(@"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$") { }
	}
	/// <summary>
	/// Handles `iri-reference` format validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression used is:
	///
	/// ```
	/// ^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?
	/// ```
	/// </remarks>
	public class IriReferenceFormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new IriReferenceFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "iri-reference";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.Draft2019_09;

		private IriReferenceFormatValidator()
			: base(@"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?") { }
	}
	/// <summary>
	/// Handles `json-pointer` format validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression used is:
	///
	/// ```
	/// ^(/(([^/~])|(~[01]))+)*/?$
	/// ```
	/// </remarks>
	public class JsonPointerFormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new JsonPointerFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "json-pointer";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.Draft2019_09;

		private JsonPointerFormatValidator()
			: base(@"^(/(([^/~])|(~[01]))+)*/?$") { }
	}
}