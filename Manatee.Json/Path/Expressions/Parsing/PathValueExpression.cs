namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class PathValueExpression<T> : ValueExpression
	{
		public PathExpression<T> Path
		{
			get { return (PathExpression<T>) Value; }
			set { Value = value; }
		}
	}
}