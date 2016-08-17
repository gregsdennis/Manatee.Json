/***************************************************************************************

	Copyright 2016 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonArrayExtensions.cs
	Namespace:		Manatee.Json
	Class Name:		JsonArrayExtensions
	Purpose:		Provides extension methods for JsonArrays.

***************************************************************************************/

using System.Linq;

namespace Manatee.Json
{
	/// <summary>
	/// Provides extension methods for <see cref="JsonArray"/>s.
	/// </summary>
	public static class JsonArrayExtensions
	{
		/// <summary>
		/// Returns a <see cref="JsonArray"/> containing only the <see cref="JsonValue"/>s of a specified type from a given <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="arr">The array to search</param>
		/// <param name="type">The type of value to return</param>
		/// <returns>A <see cref="JsonArray"/> containing only the <see cref="JsonValue"/>s of a specified type</returns>
		public static JsonArray OfType(this JsonArray arr, JsonValueType type)
		{
			if (arr == null) return null;
			var retVal = new JsonArray();
			retVal.AddRange(arr.Where(j => j.Type == type));
			return retVal;
		}
	}
}