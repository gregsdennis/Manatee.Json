namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class GroupExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '(';
		}
		public bool TryParse<TIn>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			index += 1;
			expression = new OperatorExpression {Operator = JsonPathOperator.GroupStart};
			errorMessage = null!;
			return true;
		}
	}
}
