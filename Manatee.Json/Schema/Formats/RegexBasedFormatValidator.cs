using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides base functionality for regular-expression-based format validation.
	/// </summary>
	public abstract class RegexBasedFormatValidator : IFormatValidator
	{
		private readonly Regex _pattern;

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public abstract string Format { get; }

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public abstract JsonSchemaVersion SupportedBy { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="RegexBasedFormatValidator"/> class.
		/// </summary>
		/// <param name="pattern">The regular expression for validation.</param>
		/// <param name="isCaseSensitive">Indicates whether the regular expression should consider case.</param>
		public RegexBasedFormatValidator([RegexPattern] string pattern, bool isCaseSensitive = false)
		{
			var options = RegexOptions.Compiled;
			if (!isCaseSensitive)
				options |= RegexOptions.IgnoreCase;
			_pattern = new Regex(pattern, options);
		}

		/// <summary>
		/// Validates a value.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if <paramref name="value"/> matches the format; otherwise false.</returns>
		public virtual bool Validate(JsonValue value)
		{
			return value.Type != JsonValueType.String || _pattern.IsMatch(value.String);
		}
	}
}