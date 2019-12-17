using Manatee.Json.Internal;
using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantStringExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '"' || input[index] == '\'';
		}
		public bool TryParse<TIn>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			if (!source.TryGetKey(ref index, out var value, out errorMessage))
			{
				expression = null!;
				return false;
			}

			expression = new ValueExpression(value);
			return true;
		}
	}
}
