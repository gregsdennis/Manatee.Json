using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantStringExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input[0].In('"', '\'');
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			throw new NotImplementedException();
		}
	}
}
