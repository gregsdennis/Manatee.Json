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
 
	File Name:		WildCardQuery.cs
	Namespace:		Manatee.Json.Path.Queries
	Class Name:		WildCardQuery
	Purpose:		Provides a wildcard for array queries. 

***************************************************************************************/

using System.Collections.Generic;

namespace Manatee.Json.Path.Queries
{
	internal class WildCardQuery : IJsonPathQuery
	{
		private static readonly WildCardQuery _searchInstance = new WildCardQuery(true);
		private static readonly WildCardQuery _arrayInstance = new WildCardQuery(false);

		private readonly string _string;

		public static WildCardQuery SearchInstance { get { return _searchInstance; } }
		public static WildCardQuery ArrayInstance { get { return _arrayInstance; } }

		private WildCardQuery(bool isForTrade)
		{
			_string = isForTrade ? "[*]" : "*";
		}

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json)
		{
			return json;
		}
		public override string ToString()
		{
			return _string;
		}
	}
}