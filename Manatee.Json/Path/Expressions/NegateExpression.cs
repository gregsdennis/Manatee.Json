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
 
	File Name:		NegateExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		NegateExpression
	Purpose:		Expresses the intent to negate a number.

***************************************************************************************/
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class NegateExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority { get { return 6; } }
		public ExpressionTreeNode<T> Root { get; set; }

		public override object Evaluate(T json, JsonValue root)
		{
			return -Convert.ToDouble(Root.Evaluate(json, root));
		}
		public override string ToString()
		{
			return Root is AddExpression<T> || Root is SubtractExpression<T>
					   ? string.Format("-({0})", Root)
					   : string.Format("-{0}", Root);
		}
	}
}