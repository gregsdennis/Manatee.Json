namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class SubtractExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			// Negation is handled during the Shunting-yard Algorithm loop
			return input[index] == '-';
		}
		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			index++;
			expression = new OperatorExpression { Operator = JsonPathOperator.Subtract };
			return null;
		}
	}
}