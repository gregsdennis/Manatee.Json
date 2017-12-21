namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class IsEqualExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return index + 1 < input.Length
				&& input[index] == '='
				&& input[index + 1] == '=';
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index += 2;
			node = new IsEqualExpression<T>();
			return null;
		}
	}
}