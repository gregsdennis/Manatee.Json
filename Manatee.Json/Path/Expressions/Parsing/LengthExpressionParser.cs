namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class LengthExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith(".length") && input.Length > 7 && !char.IsLetterOrDigit(input[7]);
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index += 7;
			node = new LengthExpression<T>();
			return null;
		}
	}
}