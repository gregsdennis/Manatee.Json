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
		Array = 1 << 0,
		/// <summary>
		/// Indicates the boolean type.
		/// </summary>
		[Display(Description="boolean")]
		Boolean = 1 << 1,
		/// <summary>
		/// Indicates the integer type.
		/// </summary>
		[Display(Description="integer")]
		Integer = 1 << 2,
		/// <summary>
		/// Indicates the null type.
		/// </summary>
		[Display(Description="null")]
		Null = 1 << 3,
		/// <summary>
		/// Indicates the number type.
		/// </summary>
		[Display(Description="number")]
		Number = 1 << 4,
		/// <summary>
		/// Indicates the object type.
		/// </summary>
		[Display(Description="object")]
		Object = 1 << 5,
		/// <summary>
		/// Indicates the string type.
		/// </summary>
		[Display(Description="string")]
		String = 1 << 6
	}
}
