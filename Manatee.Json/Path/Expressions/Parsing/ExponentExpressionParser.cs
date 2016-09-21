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
 
	File Name:		ExponentExpressionParser.cs
	Namespace:		Manatee.Json.Path.Expressions.Parsing
	Class Name:		ExponentExpressionParser
	Purpose:		Parses exponential expressions within JsonPath.

***************************************************************************************/
namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ExponentExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("^");
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new ExponentExpression<T>();
			return null;
		}
	}
}