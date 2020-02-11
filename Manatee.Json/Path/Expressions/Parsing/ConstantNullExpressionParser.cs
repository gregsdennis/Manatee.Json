using System;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantNullExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			if (input[index] == 'n' && index + 3 < input.Length)
				return input.IndexOf("null", index, 4, StringComparison.Ordinal) == index;

			return false;
		}
		public bool TryParse<TIn>(string source, ref int index, out JsonPathExpression? expression, out string? errorMessage)
		{
			if (source[index] == 'n')
			{
				index += 4;
				expression = new ValueExpression(JsonValue.Null);
			}
			else
			{
				expression = null!;
				errorMessage = "Value not recognized.";
				return false;
			}

			errorMessage = null;
			return true;
		}
	}
}