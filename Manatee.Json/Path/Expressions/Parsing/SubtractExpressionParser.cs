namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class SubtractExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			// Negation is handled during the Shunting-yard Algorithm loop
			return input[index] == '-';
		}

		public bool TryParse<TIn>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			index++;
			expression = new OperatorExpression { Operator = JsonPathOperator.Subtract };
			errorMessage = null!;
			return true;
		}
	}
}