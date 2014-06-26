/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		SchemaValidationError.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		SchemaValidationError
	Purpose:		Represents a single schema validation error.

***************************************************************************************/

using Manatee.Json.Internal;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Represents a single schema validation error.
	/// </summary>
	public class SchemaValidationError
	{
		/// <summary>
		/// The property or property path which failed validation.
		/// </summary>
		public string PropertyName { get; private set; }
		/// <summary>
		/// A message indicating the failure.
		/// </summary>
		public string Message { get; private set; }

		internal SchemaValidationError(string propertyName, string message)
		{
			PropertyName = propertyName;
			Message = message;
		}

		internal SchemaValidationError PrependPropertyName(string parent)
		{
			if (PropertyName.IsNullOrWhiteSpace())
				PropertyName = parent;
			else
				PropertyName = parent + "." + PropertyName;
			return this;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return PropertyName.IsNullOrWhiteSpace()
				? Message
				: string.Format("Property: {0} - {1}", PropertyName, Message);
		}
	}
}