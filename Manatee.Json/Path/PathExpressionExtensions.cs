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
 
	File Name:		PathExpressionExtensions.cs
	Namespace:		Manatee.Json.Path
	Class Name:		PathExpressionExtensions
	Purpose:		Provides extension methods which can be used within array
					and search JSON Path queries. 

***************************************************************************************/

using System;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Provides extension methods which can be used within array and search JSON Path queries.
	/// </summary>
	public static class PathExpressionExtensions
	{
		/// <summary>
		/// Specifies the length of a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="json">The array.</param>
		/// <returns>The length of the array.</returns>
		public static int Length(this JsonPathArray json)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Specifies the length of a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="json">The array.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>The length of the array.</returns>
		public static JsonPathArray Name(this JsonPathArray json, string name)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Specifies the length of a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="json">The array.</param>
		/// <returns>The length of the array.</returns>
		public static int Length(this JsonPathValue json)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property or, if its value is a boolean, whether the value is true.
		/// </summary>
		/// <param name="json">The value.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>true if the value is an object and contains key <paramref name="name"/> or if its value is true; otherwise false.</returns>
		public static bool HasProperty(this JsonPathValue json, string name)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property containing a number and retrieves its value.
		/// </summary>
		/// <param name="json">The value.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value if the property exists and is a number; otherwise null.</returns>
		public static JsonPathValue Name(this JsonPathValue json, string name)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines if an object contains a property containing a number and retrieves its value.
		/// </summary>
		/// <param name="json">The value.</param>
		/// <param name="index">The index to retreive.</param>
		/// <returns>The value if the property exists and is a number; otherwise null.</returns>
		public static JsonPathValue ArrayIndex(this JsonPathValue json, int index)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Gets the index of a value within an array.
		/// </summary>
		/// <param name="json">The value.</param>
		/// <param name="value">The query.</param>
		/// <returns>The index of the requested value or -1 if the value does not exist.</returns>
		public static int IndexOf(this JsonPathValue json, JsonValue value)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
	}
}