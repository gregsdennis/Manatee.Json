namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class IsLessThanEqualExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return index + 1 < input.Length && input[index] == '<' && input[index + 1] == '=';
		}

		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			index += 2;
			expression = new OperatorExpression { Operator = JsonPathOperator.LessThanOrEqual };
			return null;
		}
	}
}