using System;
using System.Linq;
using Manatee.Json.Parsing;
using Manatee.Json.Path.ArrayParameters;
using Manatee.Json.Path.Operators;
using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class PathExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '@' || input[index] == '$';
		}

		public bool TryParse<T>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			PathExpression<T> node;

			var isLocal = source[index] == '@';
			// Swallow this error from the path parser and assume the path just ended.
			// If it's really a syntax error, the expression parser should catch it.
			if (!JsonPathParser.TryParse(source, ref index, out var path, out errorMessage) &&
			    errorMessage != "Unrecognized JSON Path element.")
			{
				expression = null!;
				return false;
			}

			var lastOp = path.Operators.Last();
			if (lastOp is NameOperator name)
			{
				path.Operators.Remove(name);
				if (name.Name == "indexOf")
				{
					if (source[index] != '(')
					{
						errorMessage = "Expected '('.  'indexOf' operator requires a parameter.";
						expression = null!;
						return false;
					}

					index++;
					errorMessage = JsonParser.Parse(source, ref index, out var parameter, true);
					// Swallow this error from the JSON parser and assume the value just ended.
					// If it's really a syntax error, the expression parser should catch it.
					if (errorMessage != null && errorMessage != "Expected \',\', \']\', or \'}\'.")
					{
						errorMessage = $"Error parsing parameter for 'indexOf' expression: {errorMessage}.";
						expression = null!;
						return false;
					}

					if (source[index] != ')')
					{
						errorMessage = "Expected ')'.";
						expression = null!;
						return false;
					}

					index++;
					node = new IndexOfExpression<T>(path, isLocal, parameter);
				}
				else
					node = new NameExpression<T>(path, isLocal, name.Name);
			}
			else if (lastOp is LengthOperator length)
			{
				path.Operators.Remove(length);
				node = new LengthExpression<T>(path, isLocal);
			}
			else if (lastOp is ArrayOperator array)
			{
				path.Operators.Remove(array);
				var query = array.Query as SliceQuery;
				var constant = query?.Slices.FirstOrDefault()?.Index;
				if (query == null || query.Slices.Count() != 1 || !constant.HasValue)
				{
					errorMessage = "JSON Path expression indexers only support single constant values.";
					expression = null!;
					return false;
				}

				node = new ArrayIndexExpression<T>(path, isLocal, constant.Value);
			}
			else throw new NotImplementedException();

			expression = new PathValueExpression<T>(node);
			return true;
		}
	}
}