namespace Manatee.Json.Path.Expressions.Parsing
{
	internal enum JsonPathOperator
	{
		Add,
		And,
		// TODO: R# says this isn't used, but I don't want to break anything by removing it.  Run tests first.
		Constant,
		Divide,
		Exponent,
		Equal,
		GreaterThan,
		GreaterThanOrEqual,
		GroupStart,
		GroupEnd,
		LessThan,
		LessThanOrEqual,
		NotEqual,
		Modulo,
		Multiply,
		Negate,
		Not,
		Or,
		Subtract,
	}
}