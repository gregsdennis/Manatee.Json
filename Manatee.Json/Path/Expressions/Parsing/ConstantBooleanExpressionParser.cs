using System;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantBooleanExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			if (input[index] == 't' && index + 3 < input.Length)
				return input.IndexOf("true", index, 4, StringComparison.Ordinal) == index;

			if (input[index] == 'f' && index + 4 < input.Length)
				return input.IndexOf("false", index, 5, StringComparison.Ordinal) == index;

			return false;
		}
		public bool TryParse<TIn>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			if (source[index] == 't')
			{
				index += 4;
				expression = new ValueExpression(true);
			}
			else if (source[index] == 'f')
			{
				index += 5;
				expression = new ValueExpression(false);
			}
			else
			{
				expression = null!;
				errorMessage = "Boolean value not recognized.";
				return false;
			}

			errorMessage = null!;
			return true;
		}
	}
}