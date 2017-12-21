namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class DivideExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '/';
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new DivideExpression<T>();
			return null;
		}
	}
}