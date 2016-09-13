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
 
	File Name:		LengthOperator.cs
	Namespace:		Manatee.Json.Path.Operators
	Class Name:		LengthOperator
	Purpose:		Indicates that the current value should be an array and
					returns the number of items.

***************************************************************************************/
using System;
using System.Linq;
using Manatee.Json.Path.Expressions;

namespace Manatee.Json.Path.Operators
{
	internal class IndexOfOperator : IJsonPathOperator, IEquatable<IndexOfOperator>
	{
		public Expression<JsonValue, JsonArray> Parameter { get; }

		public IndexOfOperator(Expression<JsonValue, JsonArray> parameter)
		{
			Parameter = parameter;
		}

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			var parameter = Parameter.Evaluate(json, root);
			return json.Where(v => v.Type == JsonValueType.Array)
			           .Select(v => (JsonValue) v.Array.IndexOf(parameter))
			           .ToJson();
		}
		public override string ToString()
		{
			return $".indexOf({Parameter})";
		}
		public bool Equals(IndexOfOperator other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Parameter, other.Parameter);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as IndexOfOperator);
		}
		public override int GetHashCode()
		{
			return Parameter?.GetHashCode() ?? 0;
		}
	}
}