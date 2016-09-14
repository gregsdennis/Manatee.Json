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
 
	File Name:		ExponentExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		ExponentExpression
	Purpose:		Expresses the intent to raise one number to the power of another.

***************************************************************************************/
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class ExponentExpression<T> : ExpressionTreeBranch<T>, IEquatable<ExponentExpression<T>>
	{
		public override int Priority => 4;

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Convert.ToDouble(Left.Evaluate(json, root));
			var right = Convert.ToDouble(Right.Evaluate(json, root));
			return Math.Pow(left, right);
		}
		public override string ToString()
		{
			var left = Left.Priority <= Priority ? $"({Left})" : Left.ToString();
			var right = Right.Priority <= Priority ? $"({Right})" : Right.ToString();
			return $"{left}^{right}";
		}
		public bool Equals(ExponentExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ExponentExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ GetType().GetHashCode();
				return hashCode;
			}
		}
	}
}