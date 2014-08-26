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
 
	File Name:		IsEqualExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		IsEqualExpression
	Purpose:		Expresses the intent to compare two values.

***************************************************************************************/

using System;

namespace Manatee.Json.Path.Expressions
{
	internal class IsEqualExpression<T> : ExpressionTreeBranch<T>
	{
		public override int Priority { get { return 1; } }

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			if (left == null && right == null) return true;
			if (left == null || right == null) return false;
			var compare = left as IComparable;
			if (compare != null)
				return compare.CompareTo(Convert.ChangeType(right, left.GetType())) == 0;
			return left == right || left.Equals(right) || right.Equals(left);
		}
		public override string ToString()
		{
			return string.Format("{0} == {1}", Left, Right);
		}
	}
}