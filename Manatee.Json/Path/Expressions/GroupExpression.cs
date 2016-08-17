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
 
	File Name:		GroupExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		GroupExpression
	Purpose:		Expresses the intent to group an expression as a component
					in a larger expression.

***************************************************************************************/

namespace Manatee.Json.Path.Expressions
{
	internal class GroupExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority => 6;
		public ExpressionTreeNode<T> Group { get; set; }

		public override object Evaluate(T json, JsonValue root)
		{
			return Group.Evaluate(json, root);
		}
		public override string ToString()
		{
			return $"({Group})";
		}
	}
}