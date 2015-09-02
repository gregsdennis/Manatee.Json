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
 
	File Name:		SubtractExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		SubtractExpression
	Purpose:		Expresses the intent to subtract one number from another.

***************************************************************************************/

namespace Manatee.Json.Path.Expressions
{
	internal class SubtractExpression<T> : ExpressionTreeBranch<T>
	{
		public override int Priority { get { return 2; } }

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			if (!(left is double) || !(right is double)) return null;
			return (double)left - (double)right;
		}
		public override string ToString()
		{
			if (Right.Priority == Priority)
				return string.Format("{0}-({1})", Left, Right);
			return string.Format("{0}-{1}", Left, Right);
		}
	}
}