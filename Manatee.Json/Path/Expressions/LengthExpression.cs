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
 
	File Name:		LengthExpression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		LengthExpression
	Purpose:		Expresses the intent to retrieve the length of an array.

***************************************************************************************/
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class LengthExpression<T> : ExpressionTreeNode<T>
	{
		public override int Priority { get { return 5; } }

		public override object Evaluate(T json)
		{
			var array = json as JsonArray;
			if (array == null)
				throw new NotSupportedException("Length requires a JsonArray to evaluate.");
			return (double) array.Count;
		}
		public override string ToString()
		{
			return "@.length";
		}
	}
}