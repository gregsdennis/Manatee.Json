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
 
	File Name:		JsonValueParseException.cs
	Namespace:		Manatee.Json.Exceptions
	Class Name:		JsonValueParseException
	Purpose:		Thrown when an input string contains an invalid value.

***************************************************************************************/
using System;
using Manatee.Json.Enumerations;

namespace Manatee.Json.Exceptions
{
	/// <summary>
	/// Thrown when an input string contains an invalid value.
	/// </summary>
	[Serializable]
	public class JsonValueParseException : Exception
	{
		/// <summary>
		/// Creates a new instance of this exception.
		/// </summary>
		public JsonValueParseException(JsonValueType t, int index)
			: base(string.Format("Parse of type {0} failed at index {1}.", t, index)) { }
	}
}