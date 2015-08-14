/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonPathRoot.cs
	Namespace:		Manatee.Json.Path
	Class Name:		JsonPathRoot
	Purpose:		Provides methods which can be used to reference the path
					root within array and search JSON Path queries. 

***************************************************************************************/

using System;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Provides extension methods which can be used within array and search JSON Path queries.
	/// </summary>
	public static class JsonPathRoot
	{
		/// <summary>
		/// Specifies the length of a <see cref="JsonArray"/>.
		/// </summary>
		/// <returns>The length of the array.</returns>
		public static int Length()
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property or, if its value is a boolean, whether the value is true.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>true if the value is an object and contains key <paramref name="name"/> or if its value is true; otherwise false.</returns>
		public static bool HasProperty(string name)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property containing a number and retrieves its value.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value if the property exists and is a number; otherwise null.</returns>
		internal static JsonPathValue Name(string name)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property containing a number and retrieves its value.
		/// </summary>
		/// <param name="index">The index to retreive.</param>
		/// <returns>The value if the property exists and is a number; otherwise null.</returns>
		internal static JsonPathValue ArrayIndex(int index)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
	}
}