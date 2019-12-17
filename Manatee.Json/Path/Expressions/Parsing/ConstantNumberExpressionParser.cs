using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantNumberExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return char.IsDigit(input[index]);
		}
		public bool TryParse<TIn>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			if (!source.TryGetNumber(ref index, out var number, out errorMessage))
			{
				expression = null!;
				return false;
			}

			expression = new ValueExpression(number);
			errorMessage = null!;
			return true;
		}
	}
}