namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class NotExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("!") && !input.StartsWith("!=");
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new NotExpression<T>();
			return null;
		}
	}
}