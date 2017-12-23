using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantNumberExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return char.IsDigit(input[index]);
		}

		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			var error = source.GetNumber(ref index, out var number);
			if (error != null)
			{
				node = null;
				return error;
			}

			node = new ValueExpression<T> {Value = number};
			return null;
		}
	}
}