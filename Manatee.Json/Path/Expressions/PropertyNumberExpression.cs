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
 
	File Name:		PropertyNumberExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		PropertyNumberExpression
	Purpose:		Expresses the intent to retrieve a number from named
					property in an object.

***************************************************************************************/
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class PropertyNumberExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority { get { return 5; } }
		public string Name { get; set; }

		public override object Evaluate(T json)
		{
			var value = json as JsonValue;
			if (value == null)
				throw new NotSupportedException("PropertyNumber requires a JsonValue to evaluate.");
			return value.Type == JsonValueType.Object && value.Object.ContainsKey(Name) && value.Object[Name].Type == JsonValueType.Number
					   ? value.Object[Name].Number
					   : (double?)null;
		}
		public override string ToString()
		{
			return string.Format("@.{0}", Name);
		}
	}
}