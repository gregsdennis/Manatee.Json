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
	Namespace:		Manatee.Json.Path.ArrayParameters
	Class Name:		WildCardQuery
	Purpose:		Provides a wildcard for array queries. 

***************************************************************************************/
using System.Collections.Generic;

namespace Manatee.Json.Path.ArrayParameters
{
	internal class WildCardQuery : IJsonPathArrayQuery
	{
		private static readonly WildCardQuery _instance = new WildCardQuery();

		public static WildCardQuery Instance { get { return _instance; } }

		public IEnumerable<JsonValue> Find(JsonArray json)
		{
			return json;
		}
		public override string ToString()
		{
			return "*";
		}
	}
}