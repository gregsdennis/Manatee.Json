using System.ComponentModel.DataAnnotations;

namespace Manatee.Json.Schema
{
	public enum ContentEncoding
	{
		[Display(Description = "7bit")]
		SevenBit,
		[Display(Description = "8bit")]
		EightBit,
		[Display(Description = "binary")]
		Binary,
		[Display(Description = "quoted-printable")]
		QuotedPrintable,
		[Display(Description = "base64")]
		Base64,
		[Display(Description = "ietf-token")]
		IetfToken,
		[Display(Description = "x-token")]
		XToken
	}
}
