namespace Manatee.Json
{
	public static class JsonOptions
	{
		public static char PrettyPrintIndentChar { get; set; } = '\t';
		public static DuplicateKeyBehavior DuplicateKeyBehavior { get; set; }
		public static NullEqualityBehavior NullEqualityBehavior { get; set; }
		internal static bool AllowTrailingCommas { get; set; }
		public static ArrayEquality DefaultArrayEquality { get; set; }
		public static bool ThrowOnIncorrectTypeAccess { get; set; } = true;
	}
}
