namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class IsLessThanExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("<") && !input.StartsWith("<=");
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new IsLessThanExpression<T>();
			return null;
		}
	}
}