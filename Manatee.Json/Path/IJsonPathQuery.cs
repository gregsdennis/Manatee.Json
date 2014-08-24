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
 
	File Name:		IJsonPathQuery.cs
	Namespace:		Manatee.Json.Path
	Class Name:		IJsonPathQuery
	Purpose:		Defines methods required to perform a search in a JSON Path.

***************************************************************************************/
using System.Collections.Generic;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Defines methods required to perform a recursive search in a JSON Path.
	/// </summary>
	public interface IJsonPathQuery
	{
		/// <summary>
		/// Finds matching <see cref="JsonValue"/>s in a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="json">A collection of <see cref="JsonValue"/>s to search.</param>
		/// <returns>A collection of matching <see cref="JsonValue"/>s.</returns>
		IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json);
	}
}