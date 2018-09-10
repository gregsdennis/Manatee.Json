using System.ComponentModel.DataAnnotations;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines possible values for the <see cref="ContentEncodingKeyword"/>.
	/// </summary>
	public enum ContentEncoding
	{
		/// <summary>
		/// Indicates a 7-bit encoding.
		/// </summary>
		[Display(Description = "7bit")]
		SevenBit,
		/// <summary>
		/// Indicates a 8-bit encoding.
		/// </summary>
		[Display(Description = "8bit")]
		EightBit,
		/// <summary>
		/// Indicates a binary encoding.
		/// </summary>
		[Display(Description = "binary")]
		Binary,
		/// <summary>
		/// Indicates a quoted-printable encoding.
		/// </summary>
		[Display(Description = "quoted-printable")]
		QuotedPrintable,
		/// <summary>
		/// Indicates a base-64 encoding.
		/// </summary>
		[Display(Description = "base64")]
		Base64,
		/// <summary>
		/// Indicates an ietf-token encoding.
		/// </summary>
		[Display(Description = "ietf-token")]
		IetfToken,
		/// <summary>
		/// Indicates an x-token encoding.
		/// </summary>
		[Display(Description = "x-token")]
		XToken
	}
}
