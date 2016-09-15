namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class GroupExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input[0] == '(';
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			var error = JsonPathExpressionParser.Parse(source, ref index, out node);
			node?.BumpPriority();
			return error;
		}
	}
}
