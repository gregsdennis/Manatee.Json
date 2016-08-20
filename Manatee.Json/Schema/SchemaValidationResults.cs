/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		SchemaValidationResults.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		SchemaValidationResults
	Purpose:		Contains the results of schema validation.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

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