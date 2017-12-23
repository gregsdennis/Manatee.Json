namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class IsGreaterThanExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			if (input[index] != '>') return false;

			return index + 1 >= input.Length || input[index + 1] != '=';
		}

		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			index++;
			expression = new OperatorExpression { Operator = JsonPathOperator.GreaterThan };
			return null;
		}
	}
}