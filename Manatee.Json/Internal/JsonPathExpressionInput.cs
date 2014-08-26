namespace Manatee.Json.Internal
{
	internal enum JsonPathExpressionInput
	{
		OpenParenth,
		CloseParenth,
		Number,
		Plus,
		Minus,
		Star,
		Slash,
		Caret,
		Current,
		LessThan,
		Equal,
		GreaterThan,
		Bang,
		Quote,
		End
	}
}