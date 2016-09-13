namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class IsGreaterThanExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith(">") && !input.StartsWith(">=");
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new IsGreaterThanExpression<T>();
			return null;
		}
	}
}