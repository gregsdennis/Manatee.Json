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
 
	File Name:		HasPropertyExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		HasPropertyExpression
	Purpose:		Expresses the intent to search for a property with a given name.

***************************************************************************************/
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class HasPropertyExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority { get { return 5; } }
		public string Name { get; set; }

		public override object Evaluate(T json)
		{
			var value = json as JsonValue;
			if (value == null)
				throw new NotSupportedException("Length requires an array to evaluate.");
			var result = value.Type == JsonValueType.Object && value.Object.ContainsKey(Name);
			if (result && value.Object[Name].Type == JsonValueType.Boolean)
				result = value.Object[Name].Boolean;
			return result;
		}
		public override string ToString()
		{
			return string.Format("@.{0}", Name);
		}
	}
}