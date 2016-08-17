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
 
	File Name:		LengthExpressionTranslator.cs
	Namespace:		Manatee.Json.Path.Expressions.Translation
	Class Name:		LengthExpressionTranslator
	Purpose:		Translates from a LINQ Method Call Expression with a
					Length method to a LengthExpression.

***************************************************************************************/

using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class LengthExpressionTranslator : PathExpressionTranslator
	{
		public override ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			bool isLocal;
			var method = body as MethodCallExpression;
			return method == null
				       ? new LengthExpression<T>()
				       : new LengthExpression<T>
					       {
						       Path = BuildPath(method, out isLocal),
						       IsLocal = isLocal
					       };
		}
	}
}