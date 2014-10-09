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
 
	File Name:		DateTimeSerializationFormat.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		DateTimeSerializationFormat
	Purpose:		Enumeration of available formatting options for serializing
					DateTime objects.

***************************************************************************************/
namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Available formatting options for serializing DateTime objects.
	/// </summary>
	public enum DateTimeSerializationFormat
	{
		/// <summary>
		/// Output conforms to ISO 8601 formatting: YYYY-MM-DDThh:mm:ss.sTZD (e.g. 1997-07-16T19:20:30.45+01:00)
		/// </summary>
		Iso8601,
		/// <summary>
		/// Output is a string in the format "/Date([ms])/", where [ms] is the number of milliseconds
		/// since January 1, 1970 UTC.
		/// </summary>
		JavaConstructor,
		/// <summary>
		/// Output is a numeric value representing the number of milliseconds since January 1, 1970 UTC.
		/// </summary>
		Milliseconds,
		/// <summary>
		/// Output is formatted using the <see cref="JsonSerializerOptions.CustomDateTimeSerializationFormat"/> property.
		/// </summary>
		Custom,
	}
}
