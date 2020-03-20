namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ExpressionEndParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == ')' || input[index] == ']';
		}
		public bool TryParse<TIn>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			errorMessage = null!;

			if (source[index] == ']')
			{
				expression = null!;
				return true;
			}

			index += 1;
			expression = new OperatorExpression {Operator = JsonPathOperator.GroupEnd};
			return true;
		}
	}
}