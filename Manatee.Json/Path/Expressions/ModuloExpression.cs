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
 
	File Name:		ModuloExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		ModuloExpression
	Purpose:		Expresses the intent to calculate the modulo of two numbers.

***************************************************************************************/
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class ModuloExpression<T> : ExpressionTreeBranch<T>, IEquatable<ModuloExpression<T>>
	{
		protected override int BasePriority => 2;

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			if (!(left is double) || !(right is double)) return null;
			return (double) left%(double) right;
		}
		public override string ToString()
		{
			var left = Left.Priority <= Priority ? $"({Left})" : Left.ToString();
			var right = Right.Priority <= Priority ? $"({Right})" : Right.ToString();
			return $"{left}%{right}";
		}
		public bool Equals(ModuloExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ModuloExpression<T>);
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