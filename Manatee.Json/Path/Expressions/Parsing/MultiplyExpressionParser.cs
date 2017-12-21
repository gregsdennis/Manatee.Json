namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class MultiplyExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '*';
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new MultiplyExpression<T>();
			return null;
		}
	}
}