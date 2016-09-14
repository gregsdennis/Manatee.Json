using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

	internal class ExpressionEndParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input[0] == ')';
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = null;
			return null;
		}
	}
}
