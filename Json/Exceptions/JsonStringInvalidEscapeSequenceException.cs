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
 
	File Name:		JsonStringInvalidEscapeSequenceException.cs
	Namespace:		Manatee.Json.Exceptions
	Class Name:		JsonStringInvalidEscapeSequenceException
	Purpose:		Thrown when an invalid escape sequence is detected while
					parsing a string value.

***************************************************************************************/
using System;

namespace Manatee.Json.Exceptions
{
	///<summary>
	/// Thrown when an invalid escape sequence is detected while parsing a string value.
	///</summary>
	public class JsonStringInvalidEscapeSequenceException : Exception
	{
		///<summary>
		/// Creates a new instance of this exception.
		///</summary>
		public JsonStringInvalidEscapeSequenceException(string sequence, int index)
			: base(string.Format("Invalid escape sequence '{0}' found at index {1}.", sequence, index)) { }
	}
}
