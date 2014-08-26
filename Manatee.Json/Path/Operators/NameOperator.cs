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
 
	File Name:		NameOperator.cs
	Namespace:		Manatee.Json.Path.Operators
	Class Name:		NameOperator
	Purpose:		Indicates that the current value should be an object and
					a named property should be retrieved.

***************************************************************************************/
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Operators
{
	internal class NameOperator : IJsonPathOperator
	{
		private readonly string _name;

		public string Name { get { return _name; } }

		public NameOperator(string name)
		{
			_name = name;
		}

		public JsonArray Evaluate(JsonArray json)
		{
			return new JsonArray(json.Select(v => v.Type == JsonValueType.Object && v.Object.ContainsKey(Name)
				                                      ? v.Object[Name]
				                                      : null).NotNull());
		}
		public override string ToString()
		{
			return string.Format(".{0}", Name);
		}
	}
}