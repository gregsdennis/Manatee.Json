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
	[Serializable]
	public class TypeDoesNotContainPropertyException : Exception
	{
		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type { get; private set; }
		/// <summary>
		/// Gets the portion of the JSON structure which contain the invalid properties.
		/// </summary>
		public JsonValue Json { get; private set; }
		/// <summary>
		/// Initializes a new instance of the TypeDoesNotContainPropertyException class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="json">The invalid JSON structure.</param>
		public TypeDoesNotContainPropertyException(Type type, JsonValue json)
			: base(string.Format("Type {0} does not contain any properties within {1}.", type, json))
		{
			Type = type;
			Json = json;
		}
	}
}