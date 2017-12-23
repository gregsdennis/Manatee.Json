using System.Collections.Generic;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class OperatorExpression : JsonPathExpression
	{
		public JsonPathOperator Operator { get; set; }

		public bool IsBinary
		{
			get
			{
				switch (Operator)
				{
					case JsonPathOperator.Add:
					case JsonPathOperator.And:
					case JsonPathOperator.Divide:
					case JsonPathOperator.Exponent:
					case JsonPathOperator.Equal:
					case JsonPathOperator.GreaterThan:
					case JsonPathOperator.GreaterThanOrEqual:
					case JsonPathOperator.LessThan:
					case JsonPathOperator.LessThanOrEqual:
					case JsonPathOperator.NotEqual:
					case JsonPathOperator.Modulo:
					case JsonPathOperator.Multiply:
					case JsonPathOperator.Subtract:
					case JsonPathOperator.Or:
						return true;
					default:
						return false;
				}
			}
		}

		public List<JsonPathExpression> Children { get; } = new List<JsonPathExpression>();
	}
}