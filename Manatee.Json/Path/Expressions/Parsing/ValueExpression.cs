namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ValueExpression : JsonPathExpression
	{
		public object Value { get; }

		public ValueExpression(object value)
		{
			Value = value;
		}
	}
}