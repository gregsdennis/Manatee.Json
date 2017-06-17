using Manatee.Json.Path.ArrayParameters;
using Manatee.Json.Path.Expressions;
using Manatee.Json.Path.Expressions.Parsing;
using Manatee.Json.Path.Operators;

namespace Manatee.Json.Path.Parsing
{
	internal class ExpressionFilterParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("[?(");
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			index += 3;
			Expression<bool, JsonValue> expression;
			var error = JsonPathExpressionParser.Parse(source, ref index, out expression);

			if (error != null)
				return error;

			if (index >= source.Length) return "Unexpected end of input.";
			if (source[index] != ']') return "Expected ']'";
			index++;
			path.Operators.Add(new ArrayOperator(new FilterExpressionQuery(expression)));
			return null;
		}
	}
}
