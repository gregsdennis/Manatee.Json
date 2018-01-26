using System;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Thrown when a schema could not be loaded.
	/// </summary>
	public class SchemaLoadException : Exception
	{
		internal SchemaLoadException(string message) : base(message) { }
	}
}