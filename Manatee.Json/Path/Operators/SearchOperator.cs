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
 
	File Name:		SearchOperator.cs
	Namespace:		Manatee.Json.Path.Operators
	Class Name:		SearchOperator
	Purpose:		Indicates that the path should search for a named property.

***************************************************************************************/
namespace Manatee.Json.Path.Operators
{
	internal class SearchOperator : IJsonPathOperator
	{
		private readonly IJsonPathSearchParameter _parameter;

		public SearchOperator(IJsonPathSearchParameter parameter)
		{
			_parameter = parameter;
		}

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			return new JsonArray(_parameter.Find(json, root));
		}
		public override string ToString()
		{
			return $"..{_parameter}";
		}
	}
}