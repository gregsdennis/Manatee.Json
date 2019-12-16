using System;

// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations
{
	/// <summary>
	/// Indicates that parameter is regular expression pattern.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class RegexPatternAttribute : Attribute { }
	/// <summary>
	/// Indicates that the marked method builds string by the format pattern and (optional) arguments.
	/// The parameter, which contains the format string, should be given in constructor. The format string
	/// should be in <see cref="string.Format(IFormatProvider,string,object[])"/>-like form.
	/// </summary>
	/// <example><code>
	/// [StringFormatMethod("message")]
	/// void ShowError(string message, params object[] args) { /* do something */ }
	/// 
	/// void Foo() {
	///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
	/// }
	/// </code></example>
	[AttributeUsage(
		AttributeTargets.Constructor | AttributeTargets.Method |
		AttributeTargets.Property | AttributeTargets.Delegate)]
	internal sealed class StringFormatMethodAttribute : Attribute
	{
		/// <param name="formatParameterName">
		/// Specifies which parameter of an annotated method should be treated as the format string
		/// </param>
		public StringFormatMethodAttribute(string formatParameterName)
		{
			FormatParameterName = formatParameterName;
		}

		public string FormatParameterName { get; }
	}

	internal sealed class UsedImplicitlyAttribute : Attribute { }
}