namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantBooleanExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			if (input[index] == 't'
				&& index + 3 < input.Length)
			{
				return input.IndexOf("true", index, 4) == index;
			}
			else if (input[index] == 'f'
				&& index + 4 < input.Length)
			{
				return input.IndexOf("false", index, 5) == index;
			}

			return false;
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			var substring = source.Substring(index);
			if (substring.StartsWith("true"))
			{
				index += 4;
				node = new ValueExpression<T> {Value = true};
				return null;
			}
			if (substring.StartsWith("false"))
			{
				index += 5;
				node = new ValueExpression<T> {Value = false};
				return null;
			}
			node = null;
			return "Boolean value not recognized.";
		}
	}
}