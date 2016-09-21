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
 
	File Name:		PathExpressionParser.cs
	Namespace:		Manatee.Json.Path.Expressions.Parsing
	Class Name:		PathExpressionParser
	Purpose:		Parses expressions within JsonPath.

***************************************************************************************/
using System;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Parsing;
using Manatee.Json.Path.ArrayParameters;
using Manatee.Json.Path.Operators;
using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class PathExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input[0].In('@', '$');
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			var isLocal = source[index] == '@';
			JsonPath path;
			var error = JsonPathParser.Parse(source, ref index, out path);
			// Swallow this error from the path parser and assume the path just ended.
			// If it's really a syntax error, the expression parser should catch it.
			if (error != null && error != "Unrecognized JSON Path element.")
			{
				node = null;
				return error;
			}

			var name = path.Operators.Last() as NameOperator;
			var length = path.Operators.Last() as LengthOperator;
			var array = path.Operators.Last() as ArrayOperator;
			if (name != null)
			{
				path.Operators.Remove(name);
				if (name.Name == "indexOf")
				{
					JsonValue parameter;
					if (source[index] != '(')
					{
						node = null;
						return "Expected '('.  'indexOf' operator requires a parameter.";
					}
					index++;
					error = JsonParser.Parse(source, ref index, out parameter, true);
					// Swallow this error from the JSON parser and assume the value just ended.
					// If it's really a syntax error, the expression parser should catch it.
					if (error != null && error != "Expected \',\', \']\', or \'}\'.")
					{
						node = null;
						return $"Error parsing parameter for 'indexOf' expression: {error}.";
					}
					if (source[index] != ')')
					{
						node = null;
						return "Expected ')'.";
					}
					index++;
					node = new IndexOfExpression<T>
						{
							Path = path,
							IsLocal = isLocal,
							Parameter = parameter
						};
				}
				else
					node = new NameExpression<T>
						{
							Path = path,
							IsLocal = isLocal,
							Name = name.Name
						};
			}
			else if (length != null)
			{
				path.Operators.Remove(length);
				node = new LengthExpression<T>
					{
						Path = path,
						IsLocal = isLocal
					};
			}
			else if (array != null)
			{
				path.Operators.Remove(array);
				var query = array.Query as SliceQuery;
				var constant = query?.Slices.FirstOrDefault()?.Index;
				if (query == null || query.Slices.Count() != 1 || !constant.HasValue)
				{
					node = null;
					return "JSON Path expression indexers only support single constant values.";
				}
				node = new ArrayIndexExpression<T>
					{
						Path = path,
						IsLocal = isLocal,
						Index = constant.Value
					};
			}
			else
				throw new NotImplementedException();

			return null;
		}
	}
}