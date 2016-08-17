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
 
	File Name:		FieldExpressionTranslator.cs
	Namespace:		Manatee.Json.Path.Expressions.Translation
	Class Name:		FieldExpressionTranslator
	Purpose:		Translates from a LINQ Member Expression with field info
					to a FieldExpression.

***************************************************************************************/
using System.Linq.Expressions;
using System.Reflection;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class FieldExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var member = (MemberExpression)body;
			return new FieldExpression<T>
			{
				Field = (FieldInfo)member.Member,
				Source = ((ConstantExpression)member.Expression).Value
			};
		}
	}
}