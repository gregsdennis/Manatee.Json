using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantStringExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '"' || input[index] == '\'';
		}

		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			var error = source.GetKey(ref index, out var value);
			if (error != null)
			{
				node = null;
				return error;
			}

			node = new ValueExpression<T> {Value = value};
			return null;
		}
	}
}
