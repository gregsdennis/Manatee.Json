using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Contains the results of schema validation.
	/// </summary>
	public class SchemaValidationResults 
	{
		/// <summary>
		/// Gets whether the validation was successful.
		/// </summary>
		public bool Valid => !Errors.Any();

		/// <summary>
		/// Gets a collection of any errors which may have occurred during validation.
		/// </summary>
		public IEnumerable<SchemaValidationError> Errors { get; }

		internal SchemaValidationResults(string propertyName, string message)
		{
			Errors = new[] {new SchemaValidationError(propertyName, message)};
		}
		internal SchemaValidationResults(IEnumerable<SchemaValidationError> errors = null)
		{
			Errors = errors?.Distinct() ?? Enumerable.Empty<SchemaValidationError>();
		}
		internal SchemaValidationResults(IEnumerable<SchemaValidationResults> aggregate)
		{
			Errors = aggregate.SelectMany(r => r.Errors).Distinct();
		}
	}
}