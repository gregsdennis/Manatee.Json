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
 
	File Name:		TypeDoesNotContainPropertyException.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		TypeDoesNotContainPropertyException
	Purpose:		Optionally thrown when deserializing and the JSON structure
					contains property names which are not valid for the type
					requested.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Optionally thrown when deserializing and the JSON structure contains property names
	/// which are not valid for the type requested.
	/// </summary>
#if !IOS
	[Serializable]
#endif
	public class TypeDoesNotContainPropertyException : Exception
	{
		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type { get; }
		/// <summary>
		/// Gets the portion of the JSON structure which contain the invalid properties.
		/// </summary>
		public JsonValue Json { get; }
		/// <summary>
		/// Initializes a new instance of the <see cref="TypeDoesNotContainPropertyException"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="json">The invalid JSON structure.</param>
		internal TypeDoesNotContainPropertyException(Type type, JsonValue json)
			: base($"Type {type} does not contain any properties within {json}.")
		{
			Type = type;
			Json = json;
		}
	}
}