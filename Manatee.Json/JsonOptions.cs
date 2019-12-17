using System;

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

		/// <summary>
		/// Provides "verbose" level logging during serialization and schema processing.  Default is null (no logs generated).
		/// </summary>
		public static ILog? Log { get; set; }
		/// <summary>
		/// Defines the logging categories that will be generated.  Default is <see cref="LogCategory.All"/>.
		/// </summary>
		public static LogCategory LogCategory { get; set; } = LogCategory.All;
	}

	/// <summary>
	/// Enumerates the various logging categories used within the library.
	/// </summary>
	[Flags]
	public enum LogCategory
	{
		/// <summary>
		/// No category - Not used
		/// </summary>
		None,
		/// <summary>
		/// General log messages will be generated.
		/// </summary>
		General = 1 << 0,
		/// <summary>
		/// Log messages pertaining to serialization will be generated.
		/// </summary>
		Serialization = 1 << 1,
		/// <summary>
		/// Log message pertaining to schema processing will be generated.
		/// </summary>
		Schema = 1 << 2,
		/// <summary>
		/// All log messages will be generated.
		/// </summary>
		All = General | Serialization | Schema
	}
}
