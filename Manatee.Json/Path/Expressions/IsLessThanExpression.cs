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
 
	File Name:		IsLessThanExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		IsLessThanExpression
	Purpose:		Expresses the intent to compare two values.

***************************************************************************************/

namespace Manatee.Json.Path.Expressions
{
	internal class IsLessThanExpression<T> : ExpressionTreeBranch<T>
	{
		public override int Priority { get { return 1; } }

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			return ValueComparer.LessThan(left, right);
		}
		public override string ToString()
		{
			return string.Format("{0} < {1}", Left, Right);
		}
	}
}