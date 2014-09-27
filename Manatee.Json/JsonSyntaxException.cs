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
 
	File Name:		JsonSyntaxException.cs
	Namespace:		Manatee.Json
	Class Name:		JsonSyntaxException
	Purpose:		Thrown when an input string contains a syntax error other
					than a key or value parse error.

***************************************************************************************/

using System;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when an input string contains a syntax error other than a key or value parse error.
	/// </summary>
	[Serializable]
	public class JsonSyntaxException : Exception
	{
		internal JsonSyntaxException(string format, params object[] parameters)
			: base(string.Format(format, parameters)) { }
		internal JsonSyntaxException(int index)
			: base(string.Format("Parse found a syntax error at index {0}.", index)) { }
		internal JsonSyntaxException(int index, Exception innerException)
			: base(string.Format("Parse found a syntax error at index {0}.", index), innerException) { }
	}
}