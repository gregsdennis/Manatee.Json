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
 
	File Name:		Expression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		Expression
	Purpose:		Represents an expression in JsonPaths.

***************************************************************************************/
using System;
using Manatee.Json.Path.Expressions.Translation;

namespace Manatee.Json.Path.Expressions
{
	internal class Expression<T, TIn>
	{
		internal ExpressionTreeNode<TIn> Root { get; set; }

		public T Evaluate(TIn json)
		{
			return (T) Convert.ChangeType(Root.Evaluate(json), typeof (T));
		}
		public override string ToString()
		{
			return Root.ToString();
		}

		public static implicit operator Expression<T, TIn>(System.Linq.Expressions.Expression<Func<TIn, T>> source)
		{
			return ExpressionTranslator.Translate(source);
		}
	}
}
