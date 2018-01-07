namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class AddExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '+';
		}

		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			index++;
			expression = new OperatorExpression { Operator = JsonPathOperator.Add };
			return null;
		}
	}
}