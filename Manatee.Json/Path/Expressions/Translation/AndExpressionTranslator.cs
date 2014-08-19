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
 
	File Name:		AndExpressionTranslator.cs
	Namespace:		Manatee.Json.Path.Expressions.Translation
	Class Name:		AndExpressionTranslator
	Purpose:		Translates from a LINQ Binary Expression with an AND
					operation to an AndExpression. 

***************************************************************************************/
using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class AndExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var and = body as BinaryExpression;
			if (and == null)
				throw new InvalidOperationException();
			return new AndExpression<T>
			{
				Left = ExpressionTranslator.TranslateNode<T>(and.Left),
				Right = ExpressionTranslator.TranslateNode<T>(and.Right)
			};
		}
	}
}