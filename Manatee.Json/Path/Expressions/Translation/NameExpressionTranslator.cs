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
 
	File Name:		NameExpressionTranslator.cs
	Namespace:		Manatee.Json.Path.Expressions.Translation
	Class Name:		NameExpressionTranslator
	Purpose:		Translates from a LINQ Method Call Expression with a
					Name method to a NameExpression.

***************************************************************************************/
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class NameExpressionTranslator : PathExpressionTranslator
	{
		public override ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			bool isLocal;
			var method = body as MethodCallExpression;
			if (method == null)
				throw new InvalidOperationException();
			var parameter = method.Arguments.Last() as ConstantExpression;
			if (parameter == null || parameter.Type != typeof(string))
				throw new NotSupportedException("Only literal string arguments are supported in Name().");
			return new NameExpression<T>
				{
					Path = BuildPath(method, out isLocal),
					IsLocal = isLocal,
					Name = (string)parameter.Value
				};
		}
	}
}