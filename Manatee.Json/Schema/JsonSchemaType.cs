using System;
using System.ComponentModel.DataAnnotations;

namespace Manatee.Json.Schema
{
	[Flags]
	public enum JsonSchemaType
	{
		NotDefined,
		[Display(Description="array")]
		Array = 0x01,
		[Display(Description="boolean")]
		Boolean = 0x02,
		[Display(Description="integer")]
		Integer = 0x04,
		[Display(Description="null")]
		Null = 0x08,
		[Display(Description="number")]
		Number = 0x10,
		[Display(Description="object")]
		Object = 0x20,
		[Display(Description="string")]
		String = 0x40
	}
}
