namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class AndExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			if (index + 1 >= input.Length) return false;

			return input[index] == '&' && input[index + 1] == '&';
		}

		public bool TryParse<T>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			index += 2;
			expression = new OperatorExpression { Operator = JsonPathOperator.And };
			errorMessage = null!;
			return true;
		}
	}
}