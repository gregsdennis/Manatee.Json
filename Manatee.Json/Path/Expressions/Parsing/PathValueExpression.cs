namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class PathValueExpression<T> : ValueExpression
	{
		public PathValueExpression(object value)
			: base(value) { }

		public PathExpression<T> Path => (PathExpression<T>) Value;
	}
}