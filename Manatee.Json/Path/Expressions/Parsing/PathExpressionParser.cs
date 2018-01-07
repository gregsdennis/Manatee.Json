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

		public string TryParse<T>(string source, ref int index, out JsonPathExpression expression)
		{
			expression = null;
			PathExpression<T> node;

			var isLocal = source[index] == '@';
			var error = JsonPathParser.Parse(source, ref index, out JsonPath path);
			// Swallow this error from the path parser and assume the path just ended.
			// If it's really a syntax error, the expression parser should catch it.
			if (error != null && error != "Unrecognized JSON Path element.") return error;

			var lastOp = path.Operators.Last();
			if (lastOp is NameOperator name)
			{
				path.Operators.Remove(name);
				if (name.Name == "indexOf")
				{
					if (source[index] != '(')
						return "Expected '('.  'indexOf' operator requires a parameter.";

					index++;
					error = JsonParser.Parse(source, ref index, out JsonValue parameter, true);
					// Swallow this error from the JSON parser and assume the value just ended.
					// If it's really a syntax error, the expression parser should catch it.
					if (error != null && error != "Expected \',\', \']\', or \'}\'.")
						return $"Error parsing parameter for 'indexOf' expression: {error}.";

					if (source[index] != ')')
						return "Expected ')'.";

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
			else if (lastOp is LengthOperator length)
			{
				path.Operators.Remove(length);
				node = new LengthExpression<T>
					{
						Path = path,
						IsLocal = isLocal
					};
			}
			else if (lastOp is ArrayOperator array)
			{
				path.Operators.Remove(array);
				var query = array.Query as SliceQuery;
				var constant = query?.Slices.FirstOrDefault()?.Index;
				if (query == null || query.Slices.Count() != 1 || !constant.HasValue)
					return "JSON Path expression indexers only support single constant values.";

				node = new ArrayIndexExpression<T>
					{
						Path = path,
						IsLocal = isLocal,
						Index = constant.Value
					};
			}
			else throw new NotImplementedException();

			expression = new PathValueExpression<T> {Path = node};
			return null;
		}
	}
}