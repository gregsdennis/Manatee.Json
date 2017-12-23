using System.Collections.Generic;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal abstract class JsonPathExpression
	{
	}

	internal class ValueExpression : JsonPathExpression
	{
		public object Value { get; set; }
	}

	internal class PathValueExpression<T> : ValueExpression
	{
		public PathExpression<T> Path
		{
			get { return (PathExpression<T>)base.Value; }
			set { base.Value = value; }
		}
	}

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