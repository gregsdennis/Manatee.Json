using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Contains the results of schema validation.
	/// </summary>
	public class SchemaValidationResults 
	{
		public static readonly SchemaValidationResults Valid = new SchemaValidationResults();

		/// <summary>
		/// Gets whether the validation was successful.
		/// </summary>
		public bool IsValid => !Errors.Any();

		/// <summary>
		/// Gets a collection of any errors which may have occurred during validation.
		/// </summary>
		public IEnumerable<SchemaValidationError> Errors { get; }

		/// <summary>
		/// Creates an instance of <see cref="SchemaValidationResults"/>.
		/// </summary>
		/// <param name="propertyName">The name of the property that failed.</param>
		/// <param name="message">A message explaining the error.</param>
		public SchemaValidationResults(string propertyName, string message)
		{
			Errors = new[] {new SchemaValidationError(propertyName, message)};
		}
		/// <summary>
		/// Creates an instance of <see cref="SchemaValidationResults"/>.
		/// </summary>
		/// <param name="aggregate">A collection of <see cref="SchemaValidationResults"/> to aggregate together.</param>
		public SchemaValidationResults(IEnumerable<SchemaValidationResults> aggregate)
		{
			Errors = aggregate.SelectMany(r => r.Errors).Distinct().ToList();
		}
		internal SchemaValidationResults(IEnumerable<SchemaValidationError> errors = null)
		{
			Errors = errors?.Distinct() ?? Enumerable.Empty<SchemaValidationError>();
		}
		internal SchemaValidationResults(IEnumerable<string> errors)
		{
			Errors = errors?.Select(e => new SchemaValidationError(null, e));
		}
	}
}