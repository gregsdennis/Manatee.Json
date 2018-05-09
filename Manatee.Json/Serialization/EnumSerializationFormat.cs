using System.ComponentModel.DataAnnotations;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Enumerates serialization formats for enumerations.
	/// </summary>
	public enum EnumSerializationFormat
	{
		/// <summary>
		/// Instructs the serializer to convert enumeration values to their numeric counterparts.
		/// </summary>
		AsInteger,
		/// <summary>
		/// Instructs the serializer to convert enumeration values to their string counterparts.
		/// </summary>
		/// <remarks>
		/// This option will use the <see cref="DisplayAttribute"/>'s <see cref="DisplayAttribute.Description"/> property if it is present.  If the enumeration is marked with the flags attribute, the string representation will consist of a delimited list of names, the delimiter specified by <see cref="JsonSerializerOptions.FlagsEnumSeparator"/>.  Whenever a value is passed which does not have a named counterpart, the numeric value will be used.
		/// </remarks>
		AsName
	}
}