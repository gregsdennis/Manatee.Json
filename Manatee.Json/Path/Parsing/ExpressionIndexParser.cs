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
 
	File Name:		ExpressionIndexParser.cs
	Namespace:		Manatee.Json.Path.Parsing
	Class Name:		ExpressionIndexParser
	Purpose:		Parses JSON Path array components with index expresssions.

***************************************************************************************/
using Manatee.Json.Path.ArrayParameters;
using Manatee.Json.Path.Expressions;
using Manatee.Json.Path.Expressions.Parsing;
using Manatee.Json.Path.Operators;

namespace Manatee.Json.Path.Parsing
{
	internal class ExpressionIndexParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("[?(");
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			Expression<int, JsonArray> expression;
			var error = JsonPathExpressionParser.Parse(source, ref index, out expression);

			if (error != null)
				return error;

			path.Operators.Add(new ArrayOperator(new IndexExpressionQuery(expression)));
			return null;
		}
	}
}