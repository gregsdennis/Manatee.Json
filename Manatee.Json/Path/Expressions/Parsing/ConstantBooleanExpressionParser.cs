namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantBooleanExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			if (input[index] == 't' && index + 3 < input.Length)
				return input.IndexOf("true", index, 4) == index;

			if (input[index] == 'f' && index + 4 < input.Length)
				return input.IndexOf("false", index, 5) == index;

			return false;
		}
		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			expression = null;
			if (source[index] == 't')
			{
				index += 4;
				expression = new ValueExpression { Value = true };
			}
			else if (source[index] == 'f')
			{
				index += 5;
				expression = new ValueExpression {Value = false};
			}
			else return "Boolean value not recoginized.";

			return null;
		}
	}
}