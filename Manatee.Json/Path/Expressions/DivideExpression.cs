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
 
	File Name:		DivideExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		DivideExpression
	Purpose:		Expresses the intent to divide one number by another.

***************************************************************************************/

using System;

namespace Manatee.Json.Path.Expressions
{
	internal class DivideExpression<T> : ExpressionTreeBranch<T>
	{
		public override int Priority { get { return 3; } }

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Convert.ToDouble(Left.Evaluate(json, root));
			var right = Convert.ToDouble(Right.Evaluate(json, root));
			return left / right;
		}
		public override string ToString()
		{
			return string.Format("{0}/{1}", Left, Right);
		}
	}
}