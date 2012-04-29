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
 
	File Name:		LinqExtensions.cs
	Namespace:		Manatee.Json
	Class Name:		LinqExtensions
	Purpose:		LINQ for Manatee.Json

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manatee.Json
{
	/// <summary>
	/// These extension methods cover LINQ compatibility.
	/// </summary>
	public static class LinqExtensions
	{
		/// <summary>
		/// Converts an IEnumerable&lt;JsonValue&gt; returned from a LINQ query back into
		/// a JsonArray.
		/// </summary>
		/// <param name="results">An IEnumerable&lt;JsonValue&gt;</param>
		/// <returns>An equivalent JsonArray</returns>
		public static JsonArray ToJson(this IEnumerable<JsonValue> results)
		{
			var json = new JsonArray();
			json.AddRange(results);
			return json;
		}

		/// <summary>
		/// Converts an IEnumerable&lt;KeyValuePair&lt;string, JsonValue&gt;&gt; returned from a
		/// LINQ query back into a JsonObject.
		/// </summary>
		/// <param name="results">An IEnumerable&lt;KeyValuePair&lt;string, JsonValue&gt;&gt;</param>
		/// <returns>An equivalent JsonObject</returns>
		public static JsonObject ToJson(this IEnumerable<KeyValuePair<string, JsonValue>> results)
		{
			var json = new JsonObject();
			foreach (var keyValuePair in results)
			{
				json.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return json;
		}
	}
}
