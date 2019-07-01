using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Thrown when a schema reference cannot be resolved.
	/// </summary>
	public class SchemaReferenceNotFoundException : Exception
	{
		/// <summary>
		/// The location of the reference relative to the original schema root.
		/// </summary>
		public JsonPointer Location { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="SchemaReferenceNotFoundException"/> class.
		/// </summary>
		/// <param name="location">The location of the reference relative to the original schema root.</param>
		public SchemaReferenceNotFoundException(JsonPointer location)
			: base($"Cannot resolve schema referenced at '{location}'.")
		{
			Location = location;
		}
	}
}
