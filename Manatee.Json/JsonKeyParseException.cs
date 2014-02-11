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
 
	File Name:		JsonKeyParseException.cs
	Namespace:		Manatee.Json
	Class Name:		JsonKeyParseException
	Purpose:		Thrown when an input string contains an invalid key.

***************************************************************************************/

using System;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when an input string contains an invalid key.
	/// </summary>
	[Serializable]
	public class JsonKeyParseException : Exception
	{
		/// <summary>
		/// Creates a new instance of this exception.
		/// </summary>
		internal JsonKeyParseException(int index)
			: base(string.Format("Parse of key failed at index {0}.", index)) { }
	}
}