using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Schema
{
	public class SchemaReferenceNotFoundException : Exception
	{
		public JsonPointer Location { get; }

		public SchemaReferenceNotFoundException(JsonPointer location)
			: base($"Cannot resolve schema referenced by the pointer '{location}'.")
		{
			Location = location;
		}
	}
}
