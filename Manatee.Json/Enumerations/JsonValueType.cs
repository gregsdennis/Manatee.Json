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
 
	File Name:		JsonValueType.cs
	Namespace:		Manatee.Json.Enumerations
	Class Name:		JsonValueType
	Purpose:		Specifies various types of values for use in a JSON
					key:value pair.

***************************************************************************************/
namespace Manatee.Json.Enumerations
{
	/// <summary>
	/// Specifies various types of values for use in a JSON key:value pair.
	/// </summary>
	public enum JsonValueType
	{
		/// <summary>
		/// Indicates that the Json key:value pair contains a numeric value (double).
		/// </summary>
		Number,
		/// <summary>
		/// Indicates that the Json key:value pair contains a string.
		/// </summary>
		String,
		/// <summary>
		/// Indicates that the Json key:value pair contains a boolean value.
		/// </summary>
		Boolean,
		/// <summary>
		/// Indicates that the Json key:value pair contains a nested Json object.
		/// </summary>
		Object,
		/// <summary>
		/// Indicates that the Json key:value pair contains a Json array.
		/// </summary>
		Array,
		/// <summary>
		/// Indicates that the Json key:value pair contains a null value.
		/// </summary>
		Null
	}
}