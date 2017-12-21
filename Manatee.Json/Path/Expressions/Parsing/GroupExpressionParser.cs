namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class GroupExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '(';
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
