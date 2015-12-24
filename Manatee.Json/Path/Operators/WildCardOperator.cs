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
 
	File Name:		WildCardOperator.cs
	Namespace:		Manatee.Json.Path.Operators
	Class Name:		WildCardOperator
	Purpose:		Indicates that the path should return all values at the
					current level.

***************************************************************************************/
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Operators
{
	internal class WildCardOperator : IJsonPathOperator
	{
		public static WildCardOperator Instance { get; } = new WildCardOperator();

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			return new JsonArray(json.SelectMany(v =>
				{
					switch (v.Type)
					{
						case JsonValueType.Object:
							return v.Object.Values;
						case JsonValueType.Array:
							return v.Array;
						default:
							return Enumerable.Empty<JsonValue>();
					}
				}).NotNull());
		}
		public override string ToString()
		{
			return ".*";
		}
	}
}