using System;
using System.ComponentModel.DataAnnotations;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the recognized schema data types.
	/// </summary>
	[Flags]
	public enum JsonSchemaType
	{
		/// <summary>
		/// Provides a default value so that type cannot be assumed.
		/// </summary>
		NotDefined,
		/// <summary>
		/// Indicates the array type.
		/// </summary>
		[Display(Description="array")]
		Array = 0x01,
		/// <summary>
		/// Indicates the boolean type.
		/// </summary>
		[Display(Description="boolean")]
		Boolean = 0x02,
		/// <summary>
		/// Indicates the integer type.
		/// </summary>
		[Display(Description="integer")]
		Integer = 0x04,
		/// <summary>
		/// Indicates the null type.
		/// </summary>
		[Display(Description="null")]
		Null = 0x08,
		/// <summary>
		/// Indicates the number type.
		/// </summary>
		[Display(Description="number")]
		Number = 0x10,
		/// <summary>
		/// Indicates the object type.
		/// </summary>
		[Display(Description="object")]
		Object = 0x20,
		/// <summary>
		/// Indicates the string type.
		/// </summary>
		[Display(Description="string")]
		String = 0x40
	}
}
