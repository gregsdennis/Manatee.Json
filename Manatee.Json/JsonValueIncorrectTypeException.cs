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
 
	File Name:		JsonValueIncorrectTypeException.cs
	Namespace:		Manatee.Json
	Class Name:		JsonValueIncorrectTypeException
	Purpose:		Thrown when a value is accessed via the incorrect type accessor.

***************************************************************************************/

using System;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when a value is accessed via the incorrect type accessor.
	/// </summary>
#if !IOS
	[Serializable]
#endif
	public class JsonValueIncorrectTypeException : InvalidOperationException
	{
		/// <summary>
		/// The correct type for the <see cref="JsonValue"/> that threw the exception.
		/// </summary>
		public JsonValueType ValidType { get; }
		/// <summary>
		/// The type requested.
		/// </summary>
		public JsonValueType RequestedType { get; }
		/// <summary>
		/// Creates a new instance of this exception.
		/// </summary>
		internal JsonValueIncorrectTypeException(JsonValueType valid, JsonValueType requested)
			: base($"Cannot access value of type {valid} as type {requested}.")
		{
			ValidType = valid;
			RequestedType = requested;
		}
	}
}