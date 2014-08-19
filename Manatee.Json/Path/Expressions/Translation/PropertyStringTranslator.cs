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
 
	File Name:		PropertyStringTranslator.cs
	Namespace:		Manatee.Json.Path.Expressions.Translation
	Class Name:		PropertyStringTranslator
	Purpose:		Translates from a LINQ Method Call Expression with a
					PropertyString method to a PropertyStringExpression.

***************************************************************************************/
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class PropertyStringTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var method = body as MethodCallExpression;
			if (method == null)
				throw new InvalidOperationException();
			var parameter = method.Arguments.Last() as ConstantExpression;
			if (parameter == null || parameter.Type != typeof(string))
				throw new NotSupportedException("Only literal string arguments are supported in PropertyString()");
			return new PropertyStringExpression<T> { Name = parameter.Value.ToString() };
		}
	}
}