using System;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Thrown when a schema could not be loaded.
	/// </summary>
	public class SchemaLoadException : Exception
	{
		public MetaSchemaValidationResults MetaValidation { get; }

		internal SchemaLoadException(string message, MetaSchemaValidationResults metaValidation) : base(message)
		{
			MetaValidation = metaValidation;
		}
	}
}