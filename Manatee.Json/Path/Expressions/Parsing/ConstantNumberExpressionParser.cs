using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantNumberExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return char.IsDigit(input[index]);
		}
		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			expression = null;

			double? number;
			var error = source.GetNumber(ref index, out number);
			if (error != null)
			{
				return error;
			}

			expression = new ValueExpression { Value = number };
			return null;
		}
	}
}