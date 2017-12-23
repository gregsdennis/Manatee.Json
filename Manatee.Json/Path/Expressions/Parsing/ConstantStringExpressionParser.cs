using Manatee.Json.Internal;
using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantStringExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '"' || input[index] == '\'';
		}
		public string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression)
		{
			expression = null;

			string value;
			var error = source.GetKey(ref index, out value);
			if (error != null)
			{
				return error;
			}
			
			expression = new ValueExpression { Value = value };
			return null;
		}
	}
}
