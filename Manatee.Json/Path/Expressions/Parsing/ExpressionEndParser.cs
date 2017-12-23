namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ExpressionEndParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == ')' || input[index] == ']';
		}
		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			if (source[index] == ']')
			{
				expression = null;
				return null;
			}

			index += 1;
			expression = new OperatorExpression { Operator = JsonPathOperator.GroupEnd };
			return null;
		}
	}
}