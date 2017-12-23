namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class GroupExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '(';
		}
		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			index += 1;
			expression = new OperatorExpression { Operator = JsonPathOperator.GroupStart };
			return null;
		}
	}
}
