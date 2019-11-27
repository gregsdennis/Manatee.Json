namespace Manatee.Json
{
	/// <summary>
	/// Provides some configurability around the basic JSON entities.
	/// </summary>
	public static class JsonOptions
	{
		/// <summary>
		/// Determines the indention string to use when calling <see cref="JsonValue.GetIndentedString(int)"/>.
		/// The default is a single tab.
		/// </summary>
		public static string PrettyPrintIndent { get; set; } = "\t";
		/// <summary>
		/// Defines the how duplicate keys are handled for <see cref="JsonObject"/>s.
		/// The default is <see cref="Manatee.Json.DuplicateKeyBehavior.Throw"/>.
		/// </summary>
		public static DuplicateKeyBehavior DuplicateKeyBehavior { get; set; }
		/// <summary>
		/// Defines a default value for <see cref="JsonArray.EqualityStandard"/>.
		/// The default is <see cref="ArrayEquality.SequenceEqual"/>.
		/// </summary>
		public static ArrayEquality DefaultArrayEquality { get; set; }
		/// <summary>
		/// Defines whether <see cref="JsonValue"/> should throw an exception when being accessed by the
		/// wrong accessory type (e.g. accessing an array as a boolean).  The default is true.
		/// </summary>
		public static bool ThrowOnIncorrectTypeAccess { get; set; } = true;
	}
}
