namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class DivideExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '/';
		}
		public bool TryParse<TIn>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			index++;
			expression = new OperatorExpression { Operator = JsonPathOperator.Divide };
			errorMessage = null!;
			return true;
		}
	}
}